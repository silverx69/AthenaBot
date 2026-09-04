namespace AthenaBot.Plugins
{
    public interface IAthenaBotPluginHost : IPluginHost<AthenaBotPlugin>
    {
        /// <summary>
        /// Gets the IDiscordBot instance associated with this PluginHost.
        /// </summary>
        IDiscordBot Bot { get; }
    }
}
