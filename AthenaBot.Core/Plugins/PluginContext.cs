using AthenaBot.Configuration;
using AthenaBot.Plugins.Dependencies;
using System.Reflection;
using System.Runtime.Loader;

namespace AthenaBot.Plugins
{
    public class PluginContext<TPlugin> :
        AssemblyLoadContext,
        IPluginContext<TPlugin> where TPlugin : IPlugin
    {
        /// <summary>
        /// The dynamically loaded instance of the plugin
        /// </summary>
        public TPlugin Plugin { get; internal set; }

        /// <summary>
        /// The full path to the plugin assembly being loaded.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// The full path to the directory that contains the plugin assembly.
        /// </summary>
        public string PluginPath { get; private set; }


        public ModelList<CommandConfig> Commands { get; internal set; }


        internal Assembly Assembly { get; set; }

        internal PluginDependencies Dependencies { get; set; }


        public PluginContext(string name, string pluginPath)
            : base(name, true) {
            FilePath = pluginPath;
            PluginPath = Path.GetDirectoryName(pluginPath);
            Commands = [];
        }
    }
}