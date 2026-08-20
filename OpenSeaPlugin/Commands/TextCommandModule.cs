using AthenaBot;
using AthenaBot.Commands;
using Discord;
using Discord.Commands;
using System.Net;

namespace OpenSeaPlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class TextCommandModule : AthenaCommandModule
    {
        [Command("nft")]
        [Summary("Displays a collection's statistics.")]
        public async Task Stats([Summary("The slug identifier used by OpenSea.")] string id = null) {
            using var typing = Context.Channel.EnterTypingState();
            try {
                var collections = OpenSeaCommands.ValidateCollection(Context.Guild.Id, id);

                if (collections.Length == 0)
                    await ReplyAsync("Sorry, no default collection has be configured. Please specify one in the command.");
                else {
                    var embeds = new Embed[collections.Length];

                    for (int i = 0; i < embeds.Length; i++) {
                        embeds[i] = await OpenSeaCommands.GetStatsAsync(collections[i].Slug);
                        if (i < (embeds.Length - 1))
                            await Task.Delay(500);
                    }

                    await ReplyAsync(embeds: embeds);
                }
            }
            catch (HttpRequestException hex) {
                if (hex.StatusCode == HttpStatusCode.NotFound)
                    await ReplyAsync(string.Format("The collection \"{0}\" was not found.", id));
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
