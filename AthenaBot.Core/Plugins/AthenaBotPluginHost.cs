namespace AthenaBot.Plugins
{
    public class AthenaBotPluginHost :
        PluginHost<AthenaBotPlugin>,
        IAthenaBotPluginHost
    {
        IDiscordBot bot = null;

        public IDiscordBot Bot {
            get { return bot; }
            private set { OnPropertyChanged(() => bot, value); }
        }

        public AthenaBotPluginHost(IDiscordBot bot)
            : base(bot.Directories.Plugins) {
            Bot = bot;
        }

        protected override void OnPluginLoaded(PluginContext<AthenaBotPlugin> plugin) {
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

        protected override void OnPluginKilled(PluginContext<AthenaBotPlugin> plugin) {
            try {
                plugin.Plugin.OnPluginKilled();
            }
            catch (Exception ex) {
                Logging.Error(string.Format("{0}.{1}", GetType().Name, nameof(OnPluginKilled)), ex);
            }
            try {
                RaisePluginKilled(plugin);
            }
            catch (Exception ex) {
                Logging.Error("Killed::EventHandler", ex);
            }
        }
    }
}
