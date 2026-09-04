namespace AthenaBot.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// Sets the full path to the directory the plugin was loaded from (set once by the PluginHost).
        /// </summary>
        string Directory { set; }
        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        void OnPluginLoaded();
        /// <summary>
        /// Called when the plugin is killed
        /// </summary>
        void OnPluginKilled();
    }
}
