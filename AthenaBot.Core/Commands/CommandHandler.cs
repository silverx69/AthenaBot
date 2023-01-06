using AthenaBot.Configuration;
using AthenaBot.Plugins;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System.Reflection;

namespace AthenaBot.Commands
{
    public class CommandHandler
    {
        readonly DiscordBot bot;
        readonly List<PluginCommands> pluginCommands;

        public CommandService CommandService { get; private set; }

        public InteractionService InteractionService { get; private set; }

        class PluginCommands
        {
            public LoadedPlugin<DiscordBotPlugin> Plugin { get; set; }
            public List<Discord.Commands.ModuleInfo> CommandModules { get; set; }
            public List<Discord.Interactions.ModuleInfo> InteractionModules { get; set; }
            public PluginCommands() {
                CommandModules = new List<Discord.Commands.ModuleInfo>();
                InteractionModules = new List<Discord.Interactions.ModuleInfo>();
            }
            public PluginCommands(LoadedPlugin<DiscordBotPlugin> plugin)
                : this() {
                Plugin = plugin;
            }
        }

        public CommandHandler(DiscordBot bot) {
            this.bot = bot ?? throw new ArgumentNullException(nameof(bot));

            pluginCommands = new List<PluginCommands>();

            bot.Plugins.Loaded += OnPluginLoaded;
            bot.Plugins.Killed += OnPluginKilled;

            CommandService = new CommandService(new CommandServiceConfig() {
                CaseSensitiveCommands = false,
                IgnoreExtraArgs = true,
            });

            InteractionService = new InteractionService(bot.Client, new InteractionServiceConfig());
        }

        private async void OnPluginLoaded(object sender, LoadedPlugin<DiscordBotPlugin> plugin) {

            var cmds = await InstallPluginCommandsAsync(plugin);

            if (cmds.InteractionModules.Count > 0)
                foreach (var guild in bot.Client.Guilds)
                    await InteractionService.AddModulesToGuildAsync(guild.Id, false, cmds.InteractionModules.ToArray());
        }

        private async void OnPluginKilled(object sender, LoadedPlugin<DiscordBotPlugin> plugin) {
            var idx = pluginCommands.FindIndex(s => s.Plugin == plugin);
            if (idx > -1) {
                var cmds = pluginCommands[idx];
                pluginCommands.RemoveAt(idx);

                foreach (var cmd in cmds.CommandModules)
                    await CommandService.RemoveModuleAsync(cmd);

                if (cmds.InteractionModules.Count > 0) {
                    foreach (var cmd in cmds.InteractionModules)
                        await InteractionService.RemoveModuleAsync(cmd);

                    foreach (var guild in bot.Client.Guilds)
                        await InteractionService.RegisterCommandsToGuildAsync(guild.Id);
                }
            }
        }

        public async Task InstallCommandsAsync() {
            //connect client to command handler
            bot.Client.MessageReceived += HandleCommandAsync;
            bot.Client.MessageCommandExecuted += HandleInteractionAsync;
            bot.Client.SlashCommandExecuted += HandleInteractionAsync;
            bot.Client.UserCommandExecuted += HandleInteractionAsync;

            //load AthenaBot's built-in commands
            var asm = Assembly.GetExecutingAssembly();
            await CommandService.AddModulesAsync(asm, null);
            await InteractionService.AddModulesAsync(asm, null);

            //install commands defined in loaded plugin assemblies
            foreach (var plugin in bot.Plugins)
                await InstallPluginCommandsAsync(plugin);
        }

        private async Task<PluginCommands> InstallPluginCommandsAsync(LoadedPlugin<DiscordBotPlugin> plugin) {       
            var cmds = new PluginCommands(plugin);

            cmds.CommandModules.AddRange(await CommandService.AddModulesAsync(plugin.Assembly, null));
            cmds.InteractionModules.AddRange(await InteractionService.AddModulesAsync(plugin.Assembly, null));

            pluginCommands.Add(cmds);
            return cmds;
        }

        public async Task InstallInteractionsAsync() {
            foreach (var guild in bot.Client.Guilds)
                await InteractionService.RegisterCommandsToGuildAsync(guild.Id);
        }

        public async Task InstallInteractionsAsync(ulong guildId) {
            await InteractionService.RegisterCommandsToGuildAsync(guildId);
        }

        private async Task HandleInteractionAsync(SocketInteraction cmd) {
            if (cmd.User.IsBot) return;
            await InteractionService.ExecuteCommandAsync(new DiscordBotInteractionContext(bot, cmd), null);
        }

        private async Task HandleCommandAsync(SocketMessage smsg) {
            int argPos = 1;

            if (smsg is not SocketUserMessage message || message.Author.IsBot)
                return;

            if (!message.Content.StartsWithAny('!', '.', '$') &&
                !message.HasMentionPrefix(bot.Client.CurrentUser, ref argPos)) return;

            await CommandService.ExecuteAsync(new DiscordBotCommandContext(bot, message), argPos, null);
        }
    }
}