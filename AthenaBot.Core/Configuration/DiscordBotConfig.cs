namespace AthenaBot.Configuration
{
    public class DiscordBotConfig : ModelBase
    {
        public string DiscordApiKey { get; set; } = "YOUR API KEY";

        public bool UseDefaultCommands { get; set; } = true;

        public ActivityConfig Activity { get; set; }

        public ModelList<string> Plugins { get; set; } = [];

        public ModelList<CommandConfig> Commands { get; set; } = [];
    }
}
