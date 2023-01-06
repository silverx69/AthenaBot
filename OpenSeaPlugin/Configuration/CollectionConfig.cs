using AthenaBot;
using System.Text.Json.Serialization;

namespace OpenSeaPlugin.Configuration
{
    public class CollectionConfig : ModelBase
    {
        [JsonPropertyName("server")]
        public ulong Server { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }

        [JsonPropertyName("alerts")]
        public List<AlertConfig> Alerts { get; set; } = new List<AlertConfig>();
    }
}
