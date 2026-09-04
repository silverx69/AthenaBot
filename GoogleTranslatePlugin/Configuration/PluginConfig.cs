namespace GoogleTranslatePlugin.Configuration
{
    public class PluginConfig
    {
        public string APIKey { get; set; } = "YOUR API KEY";

        public List<ServerConfig> Servers { get; set; } = [];
    }
}
