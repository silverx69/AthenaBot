using System.Reflection;

namespace AthenaBot.Plugins
{
    public class LoadedPlugin<TPlugin> : ModelBase, ILoadedPlugin<TPlugin> where TPlugin : IPlugin
    {
        TPlugin plugin = default;
        bool enabled = false;
        string name = string.Empty;
        readonly Assembly assembly = null;

        public string Name {
            get { return name; }
            set { OnPropertyChanged(() => name, value); }
        }

        public TPlugin Plugin {
            get { return plugin; }
            set { OnPropertyChanged(() => plugin, value); }
        }

        public bool Enabled {
            get { return enabled; }
            set { OnPropertyChanged(() => enabled, value); }
        }

        public Assembly Assembly {
            get { return assembly; }
        }

        public LoadedPlugin(string name, TPlugin plugin, Assembly asm) {
            Name = name;
            Plugin = plugin;
            assembly = asm;
        }
    }
}