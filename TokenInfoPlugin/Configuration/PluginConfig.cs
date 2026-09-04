using AthenaBot;

namespace TokenInfoPlugin.Configuration
{
    public class PluginConfig
    {
        public string BscScanApiKey { get; set; }

        public DateTime LastTrending { get; set; }

        public ModelList<string> RecentTrending { get; set; } = [];

        public ModelList<ServerConfig> Servers { get; set; } = [];
    }
}
