using AthenaBot;

namespace TranslatePlugin.Configuration
{
    public class PluginConfig : ModelBase
    {
        public string APIKey { get; set; } = "YOUR API KEY";

        public List<ServerConfig> Servers { get; set; } = [];
    }
}
