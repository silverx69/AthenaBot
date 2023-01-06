using Discord;
using Discord.Interactions;

namespace AthenaBot.Commands
{
    public sealed class CommandModule : DiscordBotInteractionModule
    {
        [SlashCommand("listplugins", "Lists all loaded AthenaBot plugins.")]
        public async Task ListPlugins() {
            var eb = new EmbedBuilder {
                Title = "Loaded Plugins",
                Description = Context.Bot.Plugins
                    .Where(s => s.Enabled)
                    .Select(s => s.Name)
                    .Join("\r\n")
            };

            eb.WithCurrentTimestamp();

            await FollowupAsync(embed: eb.Build());
        }

        [SlashCommand("loadplugin", "Loads an AthenaBot plugin.", true)]
        public async Task LoadPlugin([Summary(description: "The name of the plugin to load.")] string name) {
            if (Context.Bot.Plugins.LoadPlugin(name))
                await FollowupAsync("Plugin loaded successfully.");
            else
                await FollowupAsync("Plugin load failed. Check log for details.");
        }

        [SlashCommand("killplugin", "Kills an AthenaBot plugin.", true)]
        public async Task KillPlugin([Summary(description: "The name of the plugin to kill.")] string name) {
            Context.Bot.Plugins.KillPlugin(name);
            await FollowupAsync("Plugin killed successfully.");
        }
    }
}
