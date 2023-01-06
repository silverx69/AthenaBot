using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class CommandConfig : ModelBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("admin")]
        public bool AdminOnly { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("roles")]
        public ModelList<string> Roles { get; set; }

        [JsonPropertyName("channels")]
        public ModelList<ChannelsConfig> Channels { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extended { get; set; }

        public CommandConfig() {
            Enabled = true;
            Roles = new ModelList<string>();
            Channels = new ModelList<ChannelsConfig>();
            Extended = new Dictionary<string, object>();
        }

        public CommandConfig(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Roles = new ModelList<string>();
            Channels = new ModelList<ChannelsConfig>();
            Extended = new Dictionary<string, object>();
        }
    }
}
