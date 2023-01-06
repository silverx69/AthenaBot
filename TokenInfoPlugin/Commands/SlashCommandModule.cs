using AthenaBot;
using AthenaBot.Commands;
using Discord;
using Discord.Interactions;
using System.Text;

namespace TokenInfoPlugin.Commands
{
    [Group("token", "TokenInfo Plugin Command Group")]
    public class SlashCommandModule : DiscordBotInteractionModule
    {
        static TokenInfoPlugin Plugin {
            get { return TokenInfoPlugin.Self; }
        }

        [SlashCommand("price", "Displays a currency's current price.")]
        public async Task Price([Summary(description: "The ID of the token to display.")] string id = null) {
            try {
                await FollowupAsync(embed: await TokenInfoCommands.GetPriceAsync(id));
            }
            catch(TokenInfoException tex) {
                await FollowupAsync(tex.Message);
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("TokenInfoPlugin", ex);
                await FollowupAsync("An error has occured during command. Check log for details.");
            }
        }

        [SlashCommand("info", "Shows detailed information about a currency.")]
        public async Task Info([Summary(description: "The ID of the token to display.")] string id = null) {
            try {
                await FollowupAsync(embed: await TokenInfoCommands.GetInfoAsync(id));
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
