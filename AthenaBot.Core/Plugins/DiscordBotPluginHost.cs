namespace AthenaBot.Plugins
{
    public class DiscordBotPluginHost :
        PluginHost<DiscordBotPlugin>,
        IDiscordBotPluginHost
    {
        IDiscordBot bot = null;

        public IDiscordBot Bot {
            get { return bot; }
            set { OnPropertyChanged(() => bot, value); }
        }

        public DiscordBotPluginHost(IDiscordBot bot)
            : base(bot.Directories.Plugins) {
            Bot = bot;
        }

        protected override void OnError(string name, string method, Exception ex) {
            Logging.Error(string.Format("{0}::{1}", name, method), ex);
            base.OnError(name, method, ex);
        }

        protected override void OnPluginLoaded(PluginContext<DiscordBotPlugin> plugin) {
            try {
                plugin.Plugin.Bot = Bot;
                plugin.Plugin.Directory = Path.Combine(BaseDirectory, plugin.Name);
                plugin.Plugin.OnPluginLoaded();
            }
            catch (Exception ex) {
                OnError(plugin, nameof(OnPluginLoaded), ex);
            }

            try {
                RaisePluginLoaded(plugin);
            }
            catch (Exception ex) {
                OnError(plugin, "Loaded::EventHandler", ex);
            }
        }

        protected override void OnPluginKilled(PluginContext<DiscordBotPlugin> plugin) {
            try {
                plugin.Plugin.OnPluginKilled();
            }
            catch (Exception ex) {
                OnError(plugin, nameof(OnPluginKilled), ex);
            }

            try {
                RaisePluginKilled(plugin);
            }
            catch (Exception ex) {
                OnError(plugin, "Killed::EventHandler", ex);
            }
        }
    }
}
