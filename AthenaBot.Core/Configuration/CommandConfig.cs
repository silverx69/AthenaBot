using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class CommandConfig : ModelBase
    {
        public string Name { get; set; }

        [JsonPropertyName("adminOnly")]
        public bool AdminOnly { get; set; }

        public bool Enabled { get; set; }

        public ModelList<string> Roles { get; set; }

        public ModelList<ChannelsConfig> Channels { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extended { get; set; }

        public CommandConfig() {
            Enabled = true;
            Roles = new ModelList<string>();
            Channels = new ModelList<ChannelsConfig>();
            Extended = new Dictionary<string, JsonElement>();
        }

        public CommandConfig(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Enabled = true;
            Roles = new ModelList<string>();
            Channels = new ModelList<ChannelsConfig>();
            Extended = new Dictionary<string, JsonElement>();
        }
    }
}
