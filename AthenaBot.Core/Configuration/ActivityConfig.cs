using Discord;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ActivityConfig : ModelBase, IActivity
    {
        public ActivityType Type { get; set; }

        public string Name { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extended { get; private set; }

        [JsonIgnore]
        public string Details { get; }

        [JsonIgnore]
        public ActivityProperties Flags { get; } = ActivityProperties.None;

        public ActivityConfig() {
            Extended = new Dictionary<string, JsonElement>();
        }

        public ActivityConfig(ActivityType type, string text)
            : this() {
            Type = type;
            Name = text;
        }
    }
}
