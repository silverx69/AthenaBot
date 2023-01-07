using AthenaBot.Commands;
using AthenaBot.Configuration;
using AthenaBot.Plugins;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace AthenaBot
{
    public class DiscordBot : ModelBase, IDiscordBot, IDisposable
    {
        Directories directories = null;
        DiscordBotConfig config = null;
        DiscordSocketClient client = null;
        IDiscordBotPluginHost plugins = null;

        CommandHandler commands;
        readonly string configFile = string.Empty;

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

        public IDiscordBotPluginHost Plugins {
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
            Plugins = new DiscordBotPluginHost(this);
        }

        public ServerConfig FindConfig(ulong guildId) {
            return Config.Servers.Find(s => s.Id == guildId);
        }

        public async Task StartAsync(GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent) {
            Config = await Persistence.LoadModelAsync<DiscordBotConfig>(configFile);

            Client = new DiscordSocketClient(new DiscordSocketConfig() {
                UseInteractionSnowflakeDate = false,//clock goes out of sync very easily!!
                GatewayIntents = gatewayIntents
            });

            Client.LoggedIn += LoggedIn;
            Client.Ready += ClientReady;
            Client.JoinedGuild += JoinedGuild;
            Client.Log += LogHandler;

            foreach (var plugin in Config.Plugins)
                Plugins.LoadPlugin(plugin);

            commands = new CommandHandler(this);
            commands.CommandService.Log += LogHandler;
            commands.InteractionService.Log += LogHandler;

            await commands.InstallCommandsAsync();
            await Client.LoginAsync(TokenType.Bot, Config.DiscordApiKey);
        }

        public async Task StopAsync() {

            await Client?.StopAsync();
            await Persistence.SaveModelAsync(Config, configFile);

            if (Plugins != null) {
                Plugins.Dispose();
                Plugins = null;
            }

            if (commands != null) {
                commands.CommandService.Log -= LogHandler;
                commands.InteractionService.Log -= LogHandler;
                commands = null;
            }

            if (Client != null) {
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

        private Task LoggedIn() {
            return Client.StartAsync();
        }

        private async Task ClientReady() {
            if (Config.Activity != null)
                await Client.SetActivityAsync(Config.Activity);
            await commands.InstallInteractionsAsync();
        }

        private async Task JoinedGuild(SocketGuild guild) {
            Config.Servers.Add(new ServerConfig(guild.Id));
            await Persistence.SaveModelAsync(Config, configFile);
            await commands.InstallInteractionsAsync(guild.Id);
        }

        private async Task LogHandler(LogMessage m) {
            if (m.Exception == null)
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
                    iex.Command,
                    iex.Context.Channel.Name,
                    iex.Message);
            else
                await Logging.ErrorAsync(m.Severity.ToLogLevel(), "Gateway", m.Exception);
        }

        public bool ValidateCommandRoles(DiscordBotCommandContext context, CommandInfo cmd) {
            var user = context.User as SocketGuildUser;
            var channel = context.Channel as SocketGuildChannel;

            ServerConfig config = FindConfig(context.Guild.Id);
            if (config == null) {
                config = new ServerConfig(context.Guild.Id);
                Config.Servers.Add(config);
            }

            CommandConfig ccmd = config.Commands.Find(s => s.Name == cmd.Aliases[0]);

            if (ccmd == null) {
                ccmd = new CommandConfig(cmd.Aliases[0]);
                config.Commands.Add(ccmd);
            }

            return ValidateCommandRoles(context.Guild, channel, user, ccmd);
        }

        public bool ValidateCommandRoles(DiscordBotInteractionContext context, ICommandInfo cmd) {
            var user = context.User as SocketGuildUser;
            var channel = context.Channel as SocketGuildChannel;

            ServerConfig config = FindConfig(context.Guild.Id);
            if (config == null) {
                config = new ServerConfig(context.Guild.Id);
                Config.Servers.Add(config);
            }

            string cname = cmd.ToString();
            CommandConfig ccmd = config.Commands.Find(s => s.Name == cname);

            if (ccmd == null) {
                ccmd = new CommandConfig(cname);
                config.Commands.Add(ccmd);
            }

            return ValidateCommandRoles(context.Guild, channel, user, ccmd);
        }

        private static bool ValidateCommandRoles(SocketGuild guild, SocketGuildChannel channel, SocketGuildUser user, CommandConfig ccmd) {

            ChannelsConfig chanConfig = ccmd.Channels.Find(s => s.Name == channel.Name);
            if (chanConfig == null) {
                chanConfig = new ChannelsConfig(channel.Name, ccmd.Enabled);
                ccmd.Channels.Add(chanConfig);
            }

            if (!chanConfig.Enabled)
                return false;

            if (ccmd.AdminOnly && !user.GuildPermissions.Administrator)
                return false;

            if (ccmd.Roles.Count > 0 &&
                !user.Roles.Contains(s => s.Guild.Id == guild.Id && ccmd.Roles.Contains(s.Name)))
                return false;

            return true;
        }
    }
}
