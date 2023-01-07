using AthenaBot;
using AthenaBot.Commands;
using Discord.Interactions;

namespace TokenInfoPlugin.Commands
{
    public class SlashCommandModule : DiscordBotInteractionModule
    {
        [SlashCommand("price", "Displays a currency's current price.")]
        public async Task Price([Summary(description: "The ID of the token to display.")] string id = null) {
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

        [SlashCommand("token", "Shows detailed information about a currency.")]
        public async Task Token([Summary(description: "The ID of the token to display.")] string id = null) {
            try {
                await FollowupAsync(embed: await TokenInfoCommands.GetInfoAsync(Context.Guild.Id, id));
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
