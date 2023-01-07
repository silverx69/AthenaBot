using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ChannelsConfig : ModelBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extended { get; set; }

        public ChannelsConfig() {
            Enabled = true;
            Extended = new Dictionary<string, object>();
        }

        public ChannelsConfig(string name, bool enabled) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Enabled = enabled;
            Extended = new Dictionary<string, object>();
        }
    }
}
