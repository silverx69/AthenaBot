using AthenaBot;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        [JsonPropertyName("anyToken")]
        public bool AnyToken { get; set; } = true;

        [JsonPropertyName("bscScanApiKey")]
        public string BscScanApiKey { get; set; }

        [JsonPropertyName("tokens")]
        public List<TokenConfig> Tokens { get; set; } = new List<TokenConfig>();
    }
}
