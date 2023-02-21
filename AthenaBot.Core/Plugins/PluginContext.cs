using AthenaBot.Plugins.Dependencies;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace AthenaBot.Plugins
{
    public class PluginContext<TPlugin> :
        AssemblyLoadContext,
        IPluginContext<TPlugin> where TPlugin : IPlugin
    {
        readonly AssemblyDependencyResolver _resolver;

        static readonly bool HaveNuget;
        static readonly string NugetPath;

        /// <summary>
        /// The full path to the plugin assembly being loaded.
        /// </summary>
        public string FilePath {
            get;
            private set;
        }

        /// <summary>
        /// The full path to the directory that contains the plugin assembly.
        /// </summary>
        public string PluginPath {
            get;
            private set;
        }

        public TPlugin Plugin {
            get;
            internal set;
        }

        internal Assembly Assembly {
            get;
            set;
        }

        static PluginContext() {
            NugetPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
            HaveNuget = Directory.Exists(NugetPath);
        }

        public PluginContext(string name, string pluginPath)
            : base(name, true) {
            PluginPath = Path.GetDirectoryName(pluginPath);
            FilePath = pluginPath;
            _resolver = new AssemblyDependencyResolver(FilePath);
        }

        protected override Assembly Load(AssemblyName aname) {
            //check file assembly is already loaded
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                if (asm.GetName().FullName == aname.FullName)
                    return asm;

            if (HaveNuget) {
                //check nuget repository for matching libraries
                var finfo = new FileInfo(Path.Combine(PluginPath, Name + ".deps.json"));
                if (finfo.Exists) {
                    PluginDependencies deps = null;

                    using (var stream = finfo.OpenRead())
                        deps = JsonSerializer.Deserialize<PluginDependencies>(stream, Json.Options);

                    foreach (var platform in deps.Targets) {
                        foreach (var target in platform.Value) {
                            if (deps.Libraries.TryGetValue(target.Key, out Library library)) {
                                if (string.IsNullOrEmpty(library.Path))
                                    continue;
                                foreach (var rt in target.Value.Runtime) {
                                    // holy crap we have the nuget dll <.<
                                    var pinfo = new FileInfo(Path.Combine(NugetPath, library.Path, rt.Key));
                                    if (pinfo.Exists &&
                                        pinfo.Name == aname.Name + pinfo.Extension &&
                                        rt.Value["fileVersion"] == aname.Version.ToString()) {
                                        return LoadFromAssemblyPath(pinfo.FullName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //resolve normally
            string assemblyPath = _resolver.ResolveAssemblyToPath(aname);
            if (string.IsNullOrEmpty(assemblyPath))
                return null;
            return LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
            //this method will generally be unused by the majority of plugins.
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (string.IsNullOrEmpty(libraryPath))
                return IntPtr.Zero;
            return LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}