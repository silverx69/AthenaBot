using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class DiscordBotConfig : ModelBase
    {
        public string DiscordApiKey { get; set; }

        public ActivityConfig Activity { get; set; }

        public ModelList<string> Plugins { get; set; }

        public ModelList<ServerConfig> Servers { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extended { get; set; }

        public DiscordBotConfig() {
            Plugins = new ModelList<string>();
            Servers = new ModelList<ServerConfig>();
            Extended = new Dictionary<string, JsonElement>();
        }
    }
}
