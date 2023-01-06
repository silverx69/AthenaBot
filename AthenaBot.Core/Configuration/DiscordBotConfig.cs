using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class DiscordBotConfig : ModelBase
    {
        [JsonPropertyName("apiKey")]
        public string DiscordApiKey { get; set; }

        [JsonPropertyName("status")]
        public string BotStatus { get; set; }

        [JsonPropertyName("plugins")]
        public ModelList<string> Plugins { get; set; }

        [JsonPropertyName("servers")]
        public ModelList<ServerConfig> Servers { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extended { get; set; }

        public DiscordBotConfig() {
            BotStatus = "War Games";
            Plugins = new ModelList<string>();
            Servers = new ModelList<ServerConfig>();
            Extended = new Dictionary<string, object>();
        }
    }
}
