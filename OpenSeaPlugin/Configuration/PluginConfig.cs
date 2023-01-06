using AthenaBot;
using System.Text.Json.Serialization;

namespace OpenSeaPlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        [JsonPropertyName("apiKey")]
        public string OpenSeaApiKey { get; set; }

        [JsonPropertyName("collections")]
        public List<CollectionConfig> Collections { get; set; }
    }
}
