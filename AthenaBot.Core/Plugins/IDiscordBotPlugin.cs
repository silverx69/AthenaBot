namespace AthenaBot.Plugins
{
    public interface IDiscordBotPlugin : IPlugin
    {
        /// <summary>
        /// Gets or sets the IDiscordBot instance hosting this plugin.
        /// </summary>
        IDiscordBot Bot { get; set; }
    }
}
