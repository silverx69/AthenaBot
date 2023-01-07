﻿namespace AthenaBot.Plugins
{
    public interface IPluginHost<TPlugin> : IReadOnlyObservableCollection<PluginContext<TPlugin>>, IDisposable
        where TPlugin : IPlugin
    {
        bool LoadPlugin(string name);
        void KillPlugin(string name);

        event PluginEventHandler<TPlugin> Loaded;
        event PluginEventHandler<TPlugin> Killed;
    }

    public delegate void PluginEventHandler<TPlugin>(object sender, PluginContext<TPlugin> plugin) where TPlugin : IPlugin;
}
