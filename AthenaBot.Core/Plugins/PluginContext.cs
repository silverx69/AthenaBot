using System.Reflection;
using System.Runtime.Loader;

namespace AthenaBot.Plugins
{
    public class PluginContext : AssemblyLoadContext
    {
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

        readonly AssemblyDependencyResolver _resolver;

        public PluginContext(string pluginPath) {
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
            if (assemblyPath == null) return null;
            return LoadFromAssemblyPath(assemblyPath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName) {
            //this method will generally be unused by the majority of plugins.
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath == null) return IntPtr.Zero;
            return LoadUnmanagedDllFromPath(libraryPath);
        }
    }
}