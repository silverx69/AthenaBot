namespace AthenaBot.Plugins
{
    public interface IDiscordBotPluginHost : IPluginHost<DiscordBotPlugin>
    {
        IDiscordBot Bot { get; set; }
    }
}
