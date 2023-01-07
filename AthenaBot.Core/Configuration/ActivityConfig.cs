using Discord;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ActivityConfig : ModelBase, IActivity
    {
        [JsonPropertyName("type")]
        public ActivityType Type { get; set; }

        [JsonPropertyName("text")]
        public string Name { get; set; }

        [JsonIgnore]
        public ActivityProperties Flags { get; } = ActivityProperties.None;

        [JsonIgnore]
        public string Details { get; }

        public ActivityConfig() { }

        public ActivityConfig(ActivityType type, string text) {
            Type = type;
            Name = text;
        }
    }
}
