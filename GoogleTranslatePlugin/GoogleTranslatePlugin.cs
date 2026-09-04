using AthenaBot;
using AthenaBot.Plugins;
using Google.Cloud.Translation.V2;
using GoogleTranslatePlugin.Configuration;

namespace GoogleTranslatePlugin
{
    public class GoogleTranslatePlugin : AthenaBotPlugin
    {
        public TranslationClient Client {
            get;
            private set;
        }

        internal PluginConfig Config {
            get;
            private set;
        }

        internal static GoogleTranslatePlugin Self {
            get;
            private set;
        }

        public GoogleTranslatePlugin() {
            Self = this;
        }

        public override void OnPluginLoaded() {
            string file = Path.Combine(Directory, "config.json");

            Config = Persistence.Load<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.Save(Config, file);

            if (!string.IsNullOrWhiteSpace(Config.APIKey) &&
                Config.APIKey != "YOUR API KEY") {
                try {
                    Client = TranslationClient.CreateFromApiKey(Config.APIKey);
                }
                catch (Exception ex) {
                    Logging.Error("GoogleTranslatePlugin", ex);
                }
            }
        }

        public override void OnPluginKilled() {
            Persistence.Save(Config, Path.Combine(Directory, "config.json"));
            Client?.Dispose();
            Client = null;
        }
    }
}