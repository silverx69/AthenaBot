namespace AthenaBot.Plugins
{
    public class DiscordBotPluginHost :
        PluginHost<DiscordBotPlugin>,
        IDiscordBotPluginHost
    {
        IDiscordBot bot = null;
        volatile bool unloading = false;

        public IDiscordBot Bot {
            get { return bot; }
            set { OnPropertyChanged(() => bot, value); }
        }

        public DiscordBotPluginHost(IDiscordBot bot)
            : base(bot.Directories.Plugins) {
            Bot = bot;
        }

        protected override void OnPluginLoaded(PluginContext<DiscordBotPlugin> plugin) {
            try {
                plugin.Plugin.Bot = Bot;
                plugin.Plugin.Directory = Path.Combine(BaseDirectory, plugin.Name);
                plugin.Plugin.OnPluginLoaded();
            }
            catch (Exception ex) {
                Logging.Error(string.Format("{0}.{1}", GetType().Name, nameof(OnPluginLoaded)), ex);
            }
            try {
                RaisePluginLoaded(plugin);
            }
            catch (Exception ex) {
                Logging.Error("Loaded::EventHandler", ex);
            }
        }

        protected override void OnPluginKilled(PluginContext<DiscordBotPlugin> plugin) {
            try {
                plugin.Plugin.OnPluginKilled();
            }
            catch (Exception ex) {
                Logging.Error(string.Format("{0}.{1}", GetType().Name, nameof(OnPluginKilled)), ex);
            }
            if (unloading) return;
            try {
                RaisePluginKilled(plugin);
            }
            catch (Exception ex) {
                Logging.Error("Killed::EventHandler", ex);
            }
        }
    }
}
