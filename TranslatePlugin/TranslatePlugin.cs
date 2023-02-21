using AthenaBot;
using AthenaBot.Plugins;
using Google.Cloud.Translation.V2;
using TranslatePlugin.Configuration;

namespace TranslatePlugin
{
    public class TranslatePlugin : DiscordBotPlugin
    {
        public TranslationClient Client {
            get;
            private set;
        }

        internal static PluginConfig Config {
            get;
            private set;
        }

        internal static TranslatePlugin Self {
            get;
            private set;
        }

        public TranslatePlugin() {
            Self = this;
        }

        public override void OnPluginLoaded() {
            string file = Path.Combine(Directory, "config.json");

            Config = Persistence.LoadModel<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.SaveModel(Config, file);

            if (!string.IsNullOrWhiteSpace(Config.TranslateAPIKey) &&
                Config.TranslateAPIKey != "YOUR API KEY") {
                try {
                    Client = TranslationClient.CreateFromApiKey(Config.TranslateAPIKey);
                }
                catch (Exception ex) {
                    Logging.Error("TranslatePlugin", ex);
                }
            }
        }

        public override void OnPluginKilled() {
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
            Client?.Dispose();
            Client = null;
        }
    }
}