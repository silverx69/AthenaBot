using System.Reflection;
using System.Runtime.Loader;

namespace AthenaBot.Plugins
{
    public class PluginContext<TPlugin> :
        AssemblyLoadContext,
        IPluginContext<TPlugin> where TPlugin : IPlugin
    {
        readonly AssemblyDependencyResolver _resolver;

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

        public PluginContext(string name, string pluginPath)
            : base(name, true) {
            PluginPath = Path.GetDirectoryName(pluginPath);
            FilePath = pluginPath;
            _resolver = new AssemblyDependencyResolver(FilePath);
        }

        protected override Assembly Load(AssemblyName assemblyName) {
            //check if the assembly we need is already loaded
            //Since AssemblyDependencyResolver just assumes everything is in the 'FilePath' folder.
            //For example, if AthenaBot.Core.dll is present in the plugin's directory,
            //this will use the one loaded in the Host application instead.
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                if (asm.GetName().FullName == assemblyName.FullName)
                    return asm;
            //resolve normally
            //Since AssemblyDependencyResolver just assumes everything is in the 'FilePath' folder.
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
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