using AthenaBot.Commands;
using AthenaBot.Configuration;
using AthenaBot.Interactions;
using AthenaBot.Plugins;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace AthenaBot
{
    public class DiscordBot : ModelBase, IDiscordBot, IDisposable
    {
        bool isready = false;
        bool isconnected = false;

        Directories directories = null;
        DiscordBotConfig config = null;
        DiscordSocketClient client = null;
        IAthenaBotPluginHost plugins = null;

        CommandHandler commands;
        readonly string configFile = string.Empty;

        public bool IsReady {
            get { return IsConnected && isready; }
            private set { OnPropertyChanged(() => isready, value); }
        }

        public bool IsConnected {
            get { return Client != null && isconnected; }
            private set { OnPropertyChanged(() => isconnected, value); }
        }

        public Directories Directories {
            get { return directories; }
            private set { OnPropertyChanged(() => directories, value); }
        }

        public DiscordBotConfig Config {
            get { return config; }
            private set { OnPropertyChanged(() => config, value); }
        }

        public DiscordSocketClient Client {
            get { return client; }
            private set { OnPropertyChanged(() => client, value); }
        }

        public IAthenaBotPluginHost Plugins {
            get { return plugins; }
            private set { OnPropertyChanged(() => plugins, value); }
        }

        public DiscordBot()
            : this(new Directories()) { }

        public DiscordBot(string appDataDirectory)
            : this(new Directories(appDataDirectory)) { }

        public DiscordBot(Directories directories) {
            Directories = directories ?? new Directories();
            configFile = Path.Combine(Directories.AppData, "config.json");
            Plugins = new AthenaBotPluginHost(this);
        }

        public void SaveConfig() {
            Persistence.Save(Config, configFile);
        }

        public async Task SaveConfigAsync() {
            await Persistence.SaveAsync(Config, configFile);
        }

        public async Task StartAsync(GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent) {
            Config = await Persistence.LoadAsync<DiscordBotConfig>(configFile);

            Client = new DiscordSocketClient(new DiscordSocketConfig() {
                UseInteractionSnowflakeDate = false,//clock goes out of sync very easily!!
                GatewayIntents = gatewayIntents
            });

            Client.Connected += Connected;
            Client.Disconnected += Disconnected;
            Client.LoggedIn += LoggedIn;
            Client.Ready += ClientReady;
            Client.JoinedGuild += JoinedGuild;
            Client.Log += LogHandler;

            foreach (var plugin in Config.Plugins)
                Plugins.LoadPlugin(plugin);

            commands = new CommandHandler(this);
            commands.CommandService.Log += LogHandler;
            commands.InteractionService.Log += LogHandler;

            await commands.InstallHandlerAsync();
            await Client.LoginAsync(TokenType.Bot, Config.DiscordApiKey);
        }

        public async Task StopAsync() {

            await Client?.StopAsync();
            await Persistence.SaveAsync(Config, configFile);

            IsConnected = false;
            IsReady = false;

            Plugins?.Dispose();
            Plugins = null;

            if (commands is not null) {
                commands.UninstallHandler();
                commands.CommandService.Log -= LogHandler;
                commands.InteractionService.Log -= LogHandler;
                commands = null;
            }

            if (Client is not null) {
                Client.Connected -= Connected;
                Client.Disconnected -= Disconnected;
                Client.LoggedIn -= LoggedIn;
                Client.Ready -= ClientReady;
                Client.JoinedGuild -= JoinedGuild;
                Client.Log -= LogHandler;
                Client.Dispose();
                Client = null;
            }
        }

        public void Dispose() {
            StopAsync().Wait();
            GC.SuppressFinalize(this);
        }

        private Task Connected() {
            IsConnected = true;
            return Task.CompletedTask;
        }

        private Task LoggedIn() {
            return Client.StartAsync();
        }

        private async Task ClientReady() {
            IsReady = true;
            if (Config.Activity is not null)
                await Client.SetActivityAsync(Config.Activity);
            await commands.InstallInteractionsAsync();
        }

        private Task Disconnected(Exception ex) {
            IsConnected = false;
            return Task.CompletedTask;
        }

        private async Task JoinedGuild(SocketGuild guild) {
            await commands.InstallInteractionsAsync(guild.Id);
        }

        private async Task LogHandler(LogMessage m) {
            if (m.Exception is null)
                await Logging.WriteLineAsync(m.Severity.ToLogLevel(), "Gateway", m.Message);
            else if (m.Exception is GatewayReconnectException gex)
                await Logging.WriteLineAsync(m.Severity.ToLogLevel(), "Gateway", gex.Message);
            else if (m.Exception is CommandException cex)
                await Logging.WriteLineAsync(
                     m.Severity.ToLogLevel(),
                     "Command",
                     "Command \"{0}\" failed to execute in channel #{1}. {2}",
                     cex.Command.Aliases[0],
                     cex.Context.Channel.Name,
                     cex.InnerException.Message);
            else if (m.Exception is InteractionException iex)
                await Logging.WriteLineAsync(
                    m.Severity.ToLogLevel(),
                    "Interaction",
                    "Interaction \"{0}\" failed to execute in channel #{1}. {2}",
                    iex.CommandInfo.Name,
                    iex.InteractionContext.Channel.Name,
                    iex.InnerException.Message);
            else
                await Logging.ErrorAsync(m.Severity.ToLogLevel(), "Gateway", m.Exception);
        }

        public bool ValidateCommandRoles(AthenaCommandContext context, CommandInfo cmd) {
            if (context.Guild == null)
                return true;

            return ValidateCommandRoles(
                context.Guild,
                context.Channel as SocketGuildChannel,
                context.User as SocketGuildUser,
                cmd.Aliases[0]);
        }

        public bool ValidateCommandRoles(AthenaInteractionContext context, ICommandInfo cmd) {
            if (context.Guild == null)
                return true;

            return ValidateCommandRoles(
                context.Guild,
                context.Channel as SocketGuildChannel,
                context.User as SocketGuildUser,
                cmd.ToString());
        }

        private bool ValidateCommandRoles(SocketGuild guild, SocketGuildChannel channel, SocketGuildUser user, string cmd) {
            var cmdConfig = config.Commands.Find(s => s.Name.Equals(cmd, StringComparison.InvariantCultureIgnoreCase));

            if (cmdConfig is null) {
                foreach (var plugin in Plugins) {
                    cmdConfig = plugin.Commands.Find(s => s.Name.Equals(cmd, StringComparison.InvariantCultureIgnoreCase));
                    if (cmdConfig is not null) break;
                }
                cmdConfig ??= new CommandConfig(cmd);
            }

            var serverConfig = cmdConfig.Servers.Find(s => s.Id == guild.Id);
            if (serverConfig is null) {
                serverConfig = new CommandServerConfig(guild.Id, true);
                cmdConfig.Servers.Add(serverConfig);
            }

            if (!serverConfig.Enabled)
                return false;

            if (serverConfig.AdminOnly && (!user.IsOwnerOf(guild) || !user.GuildPermissions.Administrator))
                return false;

            if (serverConfig.Roles.Count > 0 &&
                !user.Roles.Contains(s => s.Guild.Id == guild.Id && serverConfig.Roles.Contains(s.Name)))
                return false;

            var chanConfig = serverConfig.Channels.Find(s => s.Id == channel.Id);

            if (chanConfig is null) {
                chanConfig = new CommandChannelConfig(channel.Id, true);
                serverConfig.Channels.Add(chanConfig);
            }

            if (!chanConfig.Enabled)
                return false;

            return true;
        }
    }
}
