using AthenaBot;
using AthenaBot.Commands;
using Discord.Commands;
using System.Net;

namespace OpenSeaPlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    [Group("os"), Summary("OpenSea Plugin Command Group")]
    public class TextCommandModule : DiscordBotCommandModule
    {
        [Command("stats")]
        [Summary("Displays a collection's statistics.")]
        public async Task Stats([Summary("The slug identifier used by OpenSea.")] string slug = null) {
            using var typing = Context.Channel.EnterTypingState();
            try {
                slug = OpenSeaCommands.ValidateCollection(Context.Guild.Id, slug);

                if (string.IsNullOrEmpty(slug))
                    await ReplyAsync("Sorry, no default collection has be configured. Please specify one in the command.");
                else {
                    await ReplyAsync(embed: await OpenSeaCommands.GetStatsAsync(slug));
                }
            }
            catch (HttpRequestException hex) {
                if (hex.StatusCode == HttpStatusCode.NotFound)
                    await ReplyAsync(string.Format("The collection \"{0}\" was not found.", slug));
                else {
                    await Logging.ErrorAsync("OpenSeaPlugin", hex);
                    await ReplyAsync("An error has occured during command. Check log for details.");
                }
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("OpenSeaPlugin", ex);
                await ReplyAsync("An error has occured during command. Check log for details.");
            }
        }
    }
}
