using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ServerConfig : ModelBase
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }

        [JsonPropertyName("commands")]
        public ModelList<CommandConfig> Commands { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extended { get; set; }

        public ServerConfig() {
            Commands = new ModelList<CommandConfig>();
            Extended = new Dictionary<string, object>();
        }

        public ServerConfig(ulong id)
            : this() {
            Id = id;
        }
    }
}
