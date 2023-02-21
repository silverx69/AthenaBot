using AthenaBot;

namespace OpenSeaPlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        public string OpenSeaApiKey { get; set; }

        public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();
    }
}
