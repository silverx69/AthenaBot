namespace AthenaBot.Configuration
{
    public class DiscordBotConfig : ModelBase
    {
        public string DiscordApiKey { get; set; }

        public ActivityConfig Activity { get; set; }

        public ModelList<string> Plugins { get; set; }

        public ModelList<ServerConfig> Servers { get; set; }

        public DiscordBotConfig() {
            Plugins = [];
            Servers = [];
        }
    }
}
