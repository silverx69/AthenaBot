using AthenaBot.Configuration;
using AthenaBot.Plugins.Dependencies;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace AthenaBot.Plugins
{
    public abstract class PluginHost<TPlugin> :
        ModelReadOnlyList<PluginContext<TPlugin>>,
        IPluginHost<TPlugin>
        where TPlugin : IPlugin
    {
        volatile bool unloading = false;

        static readonly bool HaveNuget;
        static readonly string NugetPath;

        public string BaseDirectory {
            get;
            private set;
        }

        static PluginHost() {
            NugetPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
            HaveNuget = Directory.Exists(NugetPath);
        }

        public PluginHost(string baseDirectory) {
            BaseDirectory = baseDirectory;
        }

        public virtual void Dispose() {
            KillAllPlugins();
            GC.SuppressFinalize(this);
        }

        public bool IsPluginLoaded(string name) {
            if (name.EndsWith(".dll"))
                name = name[0..^4];

            lock (SyncRoot) {
                int index = this.FindIndex(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (index > -1) return true;
            }

            return false;
        }

        public bool LoadPlugin(string name) {
            if (name.EndsWith(".dll"))
                name = name[0..^4];

            if (IsPluginLoaded(name))
                return true;
            try {
                return LoadPluginInternal(name);
            }
            catch (Exception ex) {
                Logging.Error(string.Format("{0}.{1}", GetType().Name, nameof(LoadPlugin)), ex);
            }
            return false;
        }

        public async Task<bool> LoadPluginAsync(string name) {
            if (name.EndsWith(".dll"))
                name = name[0..^4];

            if (IsPluginLoaded(name))
                return true;
            try {
                return await Task.Run(() => LoadPluginInternal(name));
            }
            catch (Exception ex) {
                await Logging.ErrorAsync(string.Format("{0}.{1}", GetType().Name, nameof(LoadPlugin)), ex);
            }
            return false;
        }

        private bool LoadPluginInternal(string name) {
            var context = GetPluginContext<TPlugin>(name);

            var cmdFile = new FileInfo(Path.Combine(context.PluginPath, "commands.json"));
            if (cmdFile.Exists) {
                using var stream = cmdFile.OpenRead();
                context.Commands = JsonSerializer.Deserialize<ModelList<CommandConfig>>(stream, Json.Options);
            }

            var depsFile = new FileInfo(Path.Combine(context.PluginPath, name + ".deps.json"));
            if (depsFile.Exists) {
                using var stream = depsFile.OpenRead();
                context.Dependencies = JsonSerializer.Deserialize<PluginDependencies>(stream, Json.Options);
            }

            context.Assembly = context.LoadFromAssemblyPath(context.FilePath);
            context.Resolving += ResolvePluginDependency;

            Type impl = null;
            Type pluginType = typeof(TPlugin);

            foreach (var type in context.Assembly.GetExportedTypes()) {
                if (pluginType.IsAssignableFrom(type))
                    impl = type;
            }

            if (impl == null)
                throw new PluginLoadException("Assembly does not contain a valid IPlugin implementation.");

            context.Plugin = (TPlugin)Activator.CreateInstance(impl);

            lock (SyncRoot) InnerList.Add(context);

            OnPluginLoaded(context);

            return true;
        }

        private Assembly ResolvePluginDependency(AssemblyLoadContext ctx, AssemblyName aname) {
            var context = (PluginContext<TPlugin>)ctx;
            string path = Path.Combine(context.PluginPath, aname.Name + ".dll");

            // check if the assembly exists in the plugin directory
            if (File.Exists(path))
                return context.LoadFromAssemblyPath(path);

            // check if the assembly exists in the local user's nuget cache
            else if (HaveNuget && context.Dependencies is not null) {
                foreach (var platform in context.Dependencies.Targets) {
                    foreach (var target in platform.Value) {
                        if (context.Dependencies.Libraries.TryGetValue(target.Key, out Library library) && library.Type == "package") {
                            foreach (var rt in target.Value.Runtime) {
                                var pinfo = new FileInfo(Path.Combine(NugetPath, library.Path, rt.Key));
                                if (pinfo.Exists &&
                                    pinfo.Name == aname.Name + pinfo.Extension &&
                                    rt.Value["fileVersion"] == aname.Version.ToString()) {
                                    return context.LoadFromAssemblyPath(pinfo.FullName);
                                }
                            }
                        }
                    }
                }
                // download from nuget?
            }
            return null;
        }

        public void KillPlugin(string name) {
            lock (SyncRoot) {
                string lowname = name.ToLower();

                int index = this.FindIndex(s => s.Name.Equals(lowname, StringComparison.InvariantCultureIgnoreCase));
                if (index == -1) return;

                KillPlugin(this[index]);
            }
        }

        public Task KillPluginAsync(string name) {
            return Task.Run(() => KillPlugin(name));
        }

        protected void KillPlugin(PluginContext<TPlugin> context) {
            InnerList.Remove(context);
            OnPluginKilled(context);
            context.Resolving -= ResolvePluginDependency;
            context.Unload();
        }

        protected void KillAllPlugins() {
            unloading = true;
            foreach (var context in this) {
                OnPluginKilled(context);
                context.Unload();
            }
            InnerList.Clear();
            unloading = false;
        }

        protected abstract void OnPluginLoaded(PluginContext<TPlugin> context);

        protected abstract void OnPluginKilled(PluginContext<TPlugin> context);


        protected void RaisePluginLoaded(PluginContext<TPlugin> context) {
            Loaded?.Invoke(context);
        }

        protected void RaisePluginKilled(PluginContext<TPlugin> context) {
            if (!unloading) Killed?.Invoke(context);
        }

        protected virtual PluginContext<T> GetPluginContext<T>(string name) where T : IPlugin {
            return new PluginContext<T>(name, Path.Combine(BaseDirectory, name, name + ".dll"));
        }

        public event PluginEventHandler<TPlugin> Loaded;
        public event PluginEventHandler<TPlugin> Killed;
    }
}