namespace AthenaBot.Plugins
{
    public abstract class DiscordBotPlugin : IDiscordBotPlugin
    {
        /// <summary>
        /// Gets or sets the IDiscordBot instance hosting this plugin.
        /// </summary>
        public IDiscordBot Bot { get; set; }
        /// <summary>
        /// Gets / sets the full path to the directory the plugin was loaded from. 
        /// Set by the PluginHost so the IPlugin knows where it was loaded from, has no effect if modified.
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// Called when the plugin is loaded. Not used by Host application, this is for simplicity purposes.
        /// </summary>
        public virtual void OnPluginLoaded() { }
        /// <summary>
        /// Called when the plugin is killed
        /// </summary>
        public virtual void OnPluginKilled() { }
        /// <summary>
        /// Occurs when an unhandled exception occurs in any plugin (all plugins receive notification [for debugging?])
        /// </summary>
        public virtual void OnError(IErrorInfo error) { }
    }
}
