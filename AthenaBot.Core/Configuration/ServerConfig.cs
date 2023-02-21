using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Configuration
{
    public class ServerConfig : ModelBase
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }

        public ModelList<CommandConfig> Commands { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement> Extended { get; set; }

        public ServerConfig() {
            Commands = new ModelList<CommandConfig>();
            Extended = new Dictionary<string, JsonElement>();
        }

        public ServerConfig(ulong id)
            : this() {
            Id = id;
        }
    }
}
