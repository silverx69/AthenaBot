namespace AthenaBot.Plugins
{
    public abstract class AthenaBotPlugin : IAthenaBotPlugin
    {
        /// <summary>
        /// Gets or sets the IDiscordBot instance associated with this plugin (set once by the PluginHost).
        /// </summary>
        public IDiscordBot Bot { get; set; }
        /// <summary>
        /// Gets / sets the full path to the directory the plugin was loaded from (set once by the PluginHost).
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        public virtual void OnPluginLoaded() { }
        /// <summary>
        /// Called when the plugin is killed.
        /// </summary>
        public virtual void OnPluginKilled() { }
    }
}