namespace AthenaBot.Plugins
{
    public interface IPluginHost<TPlugin> : IReadOnlyObservableCollection<PluginContext<TPlugin>>, IDisposable
        where TPlugin : IPlugin
    {
        /// <summary>
        /// Tries to load a plugin with the given name into this PluginHost.
        /// </summary>
        /// <param name="name">The name of the plugin to load.</param>
        /// <returns>True if the plugin was loaded successfully or already loaded; otherwise, false.</returns>
        bool LoadPlugin(string name);

        /// <summary>
        /// Attempts to kill a plugin with the given name, removing it from this PluginHost.
        /// </summary>
        /// <param name="name">The name of the plugin to kill.</param>
        void KillPlugin(string name);

        /// <summary>
        /// Occurs when a plugin is successfully loaded into this PluginHost.
        /// </summary>
        event PluginEventHandler<TPlugin> Loaded;

        /// <summary>
        /// Occurs when a plugin is successfully killed and removed from this PluginHost.
        /// </summary>
        event PluginEventHandler<TPlugin> Killed;
    }

    public delegate void PluginEventHandler<TPlugin>(PluginContext<TPlugin> plugin) where TPlugin : IPlugin;
}
