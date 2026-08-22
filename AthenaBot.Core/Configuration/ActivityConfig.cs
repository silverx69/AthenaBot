using Discord;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ActivityConfig : ModelBase, IActivity
    {
        public ActivityType Type { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Details { get; }

        [JsonIgnore]
        public ActivityProperties Flags { get; } = ActivityProperties.None;

        public ActivityConfig() { }

        public ActivityConfig(ActivityType type, string text) {
            Type = type;
            Name = text;
        }
    }
}
