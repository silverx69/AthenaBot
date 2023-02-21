using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ChannelsConfig : ModelBase
    {
        public ulong Id { get; set; }

        public bool Enabled { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extended { get; set; }

        public ChannelsConfig() {
            Enabled = true;
            Extended = new Dictionary<string, JsonElement>();
        }

        public ChannelsConfig(ulong id, bool enabled) {
            if (id == 0)
                throw new ArgumentException("Invalid channel identifier.", nameof(id));
            Id = id;
            Enabled = enabled;
            Extended = new Dictionary<string, JsonElement>();
        }
    }
}
