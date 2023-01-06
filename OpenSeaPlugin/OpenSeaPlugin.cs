using AthenaBot;
using AthenaBot.Plugins;
using OpenSeaPlugin.Configuration;

namespace OpenSeaPlugin
{
    public class OpenSeaPlugin : DiscordBotPlugin
    {
        internal static PluginConfig Config {
            get;
            private set;
        }

        internal static OpenSeaPlugin Self { get; private set; }

        public OpenSeaPlugin() { Self = this; }

        public override void OnPluginLoaded() {
            Config = Persistence.LoadModel<PluginConfig>(Path.Combine(Directory, "config.json"));
            //Bot.Client.MessageReceived += Client_MessageReceived;
        }

        public override void OnPluginKilled() {
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
            //Bot.Client.MessageReceived -= Client_MessageReceived;
        }
    }
}