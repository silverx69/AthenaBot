using System.Collections.Specialized;
using System.ComponentModel;

namespace AthenaBot.Plugins
{
    public interface IPluginHost<TPlugin> : IReadOnlyObservableCollection<LoadedPlugin<TPlugin>> 
        where TPlugin : IPlugin
    {
        bool LoadPlugin(string name);
        void KillPlugin(string name);

        event PluginEventHandler<TPlugin> Loaded;
        event PluginEventHandler<TPlugin> Killed;
    }

    public delegate void PluginEventHandler<TPlugin>(object sender, LoadedPlugin<TPlugin> plugin) where TPlugin : IPlugin;
}
