using AthenaBot;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        [JsonPropertyName("bscScanApiKey")]
        public string BscScanApiKey { get; set; }

        [JsonPropertyName("servers")]
        public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();
    }
}
