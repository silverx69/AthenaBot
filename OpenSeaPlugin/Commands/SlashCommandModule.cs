using AthenaBot;
using AthenaBot.Commands;
using Discord.Interactions;
using System.Net;

namespace OpenSeaPlugin.Commands
{
    [Group("os", "OpenSea Plugin Command Group")]
    public class SlashCommandModule : DiscordBotInteractionModule
    {
        [SlashCommand("stats", "Displays a collection's statistics.")]
        public async Task Stats([Summary("The slug identifier used by OpenSea.")] string slug = null) {
            try {
                slug = OpenSeaCommands.ValidateCollection(Context.Guild.Id, slug);

                if (string.IsNullOrEmpty(slug))
                    await FollowupAsync("Sorry, no default collection has be configured. Please specify one in the command.");
                else {
                    await FollowupAsync(embed: await OpenSeaCommands.GetStatsAsync(slug));
                }
            }
            catch (HttpRequestException hex) {
                if (hex.StatusCode == HttpStatusCode.NotFound)
                    await FollowupAsync(string.Format("The collection with slug \"{0}\" was not found.", slug));
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
