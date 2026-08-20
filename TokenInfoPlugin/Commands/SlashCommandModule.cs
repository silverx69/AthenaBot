using AthenaBot;
using AthenaBot.Interactions;
using Discord.Interactions;

namespace TokenInfoPlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class SlashCommandModule : AthenaInteractionModule
    {
        [SlashCommand("price", "Displays a currency's current price information.")]
        public async Task Price([Summary(description: "The ID of the token to display.")] string id = null) {
            await DeferAsync();
            try {
                await FollowupAsync(embed: await TokenInfoCommands.GetPriceAsync(Context.Guild.Id, id));
            }
            catch (TokenInfoException tex) {
                await FollowupAsync(tex.Message);
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("TokenInfoPlugin", ex);
                await FollowupAsync("An error has occured during command. Check log for details.");
            }
        }
    }
}
