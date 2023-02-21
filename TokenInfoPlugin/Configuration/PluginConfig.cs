using AthenaBot;

namespace TokenInfoPlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        public string BscScanApiKey { get; set; }

        public DateTime LastTrending { get; set; }

        public List<string> RecentTrending { get; set; }

        public List<ServerConfig> Servers { get; set; }

        public PluginConfig() {
            RecentTrending = new List<string>();
            Servers = new List<ServerConfig>();
        }
    }
}
