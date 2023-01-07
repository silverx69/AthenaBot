using System.Runtime.Loader;

namespace AthenaBot.Plugins
{
    public abstract class PluginHost<TPlugin> :
        ModelReadOnlyList<PluginContext<TPlugin>>,
        IPluginHost<TPlugin>
        where TPlugin : IPlugin
    {
        public string BaseDirectory {
            get;
            private set;
        }

        public PluginHost(string baseDirectory) {
            BaseDirectory = baseDirectory;
        }

        public bool LoadPlugin(string name) {
            string lowname = name.ToLower();

            if (lowname.EndsWith(".dll")) {
                name = name[0..^4];
                lowname = lowname[0..^4];
            }

            lock (SyncRoot) {
                int index = this.FindIndex(s => s.Name.ToLower() == lowname);
                if (index > -1) return true;
            }

            try {
                var context = GetPluginContext<TPlugin>(name);
                if (context == null) return false;

                context.Assembly = context.LoadFromAssemblyPath(context.FilePath);

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
            catch (PluginLoadException plex) {
                OnError(GetType().Name, nameof(LoadPlugin), plex);
            }
            catch (Exception ex) {
                OnError(GetType().Name, nameof(LoadPlugin), ex);
            }
            return false;
        }

        public void KillPlugin(string name) {
            PluginContext<TPlugin> context;

            lock (SyncRoot) {
                string lowname = name.ToLower();

                int index = this.FindIndex(s => s.Name.ToLower() == lowname);
                if (index == -1) return;

                context = this[index];
                InnerList.RemoveAt(index);
            }

            context.Unloading += OnPluginUnloading;
            context.Unload();

            OnPluginKilled(context);
        }

        protected abstract void OnPluginLoaded(PluginContext<TPlugin> context);

        protected abstract void OnPluginKilled(PluginContext<TPlugin> context);

        //Occurs when the plugin assembly is actually unloaded by the runtime
        protected virtual async void OnPluginUnloading(AssemblyLoadContext ctx) {
            ctx.Unloading -= OnPluginUnloading;
            await Logging.InfoAsync("PluginHost", "The plugin \"{0}\" has been unloaded.", ctx.Name);
        }


        protected virtual void OnError(PluginContext<TPlugin> context, string method, Exception ex) {
            OnError(context.Name, method, ex);
        }

        protected virtual void OnError(string name, string method, Exception ex) {
            var error = new PluginErrorInfo(name, method, ex);

            foreach (var context in this) {
                try {
                    context.Plugin.OnError(error);
                }
                catch {
                    /* do not route back into the plugins.. possible stack overflow */
                }
            }
        }


        protected void RaisePluginLoaded(PluginContext<TPlugin> context) {
            Loaded?.Invoke(this, context);
        }

        protected void RaisePluginKilled(PluginContext<TPlugin> context) {
            Killed?.Invoke(this, context);
        }

        protected virtual PluginContext<T> GetPluginContext<T>(string dllname) where T : IPlugin {
            return new PluginContext<T>(dllname, Path.Combine(BaseDirectory, dllname, dllname + ".dll"));
        }

        public event PluginEventHandler<TPlugin> Loaded;
        public event PluginEventHandler<TPlugin> Killed;
    }
}