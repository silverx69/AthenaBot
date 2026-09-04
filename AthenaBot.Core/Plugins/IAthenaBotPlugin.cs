namespace AthenaBot.Plugins
{
    public interface IAthenaBotPlugin : IPlugin
    {
        /// <summary>
        /// Sets the IDiscordBot instance associated with this plugin (set once by the PluginHost).
        /// </summary>
        IDiscordBot Bot { set; }
    }
}
