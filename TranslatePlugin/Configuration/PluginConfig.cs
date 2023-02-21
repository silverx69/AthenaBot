using AthenaBot;

namespace TranslatePlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        public string TranslateAPIKey { get; set; } = "YOUR API KEY";

        public List<ServerConfig> Servers { get; set; }

        public PluginConfig() {
            Servers = new List<ServerConfig>();
        }
    }
}
