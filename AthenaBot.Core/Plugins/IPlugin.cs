namespace AthenaBot.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// Sets the full path to the directory the plugin was loaded from. 
        /// Set by the PluginHost so the IPlugin knows where it was loaded from, has no effect if modified.
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
