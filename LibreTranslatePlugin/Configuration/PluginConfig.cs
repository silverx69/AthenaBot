using AthenaBot;

namespace LibreTranslatePlugin.Configuration
{
    public class PluginConfig
    {
        public string APIUrl { get; set; } = "http://localhost:5000";
        public string APIKey { get; set; } = "YOUR API KEY";

        public ModelList<ServerConfig> Servers { get; set; } = [];
    }
}