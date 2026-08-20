using AthenaBot;
using AthenaBot.Interactions;
using Discord;
using Discord.Interactions;
using System.Net;

namespace OpenSeaPlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class SlashCommandModule : AthenaInteractionModule
    {
        [SlashCommand("nft", "Displays a collection's statistics.")]
        public async Task Stats([Summary(description: "The slug identifier used by OpenSea.")] string id = null) {
            await DeferAsync();
            try {
                var collections = OpenSeaCommands.ValidateCollection(Context.Guild.Id, id);

                if (collections.Length == 0)
                    await FollowupAsync("Sorry, no default collection has be configured. Please specify one in the command.");
                else {
                    var embeds = new Embed[collections.Length];

                    for (int i = 0; i < embeds.Length; i++) {
                        embeds[i] = await OpenSeaCommands.GetStatsAsync(collections[i].Slug);
                        if (i < (embeds.Length - 1))
                            await Task.Delay(500);
                    }

                    await FollowupAsync(embeds: embeds);
                }
            }
            catch (HttpRequestException hex) {
                if (hex.StatusCode == HttpStatusCode.NotFound)
                    await FollowupAsync(string.Format("The collection with slug \"{0}\" was not found.", id));
                else {
                    await Logging.ErrorAsync("OpenSeaPlugin", hex);
                    await FollowupAsync("An error has occured during command. Check log for details.");
                }
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("OpenSeaPlugin", ex);
                await FollowupAsync("An error has occured during command. Check log for details.");
            }
        }
    }
}
