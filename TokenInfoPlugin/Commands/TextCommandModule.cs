using AthenaBot;
using AthenaBot.Commands;
using Discord;
using Discord.Commands;
using System.Text;

namespace TokenInfoPlugin.Commands
{
    [Group("token")]
    [Summary("TokenInfo Plugin Command Group")]
    public class TextCommandModule : DiscordBotCommandModule
    {
        static TokenInfoPlugin Plugin {
            get { return TokenInfoPlugin.Self; }
        }

        [Command("price")]
        [Summary("Displays a currency's current price.")]
        public async Task Price([Summary("The ID of the token to display.")] string id = null) {
            using var typing = Context.Channel.EnterTypingState();
            try {
                await ReplyAsync(embed: await TokenInfoCommands.GetPriceAsync(id));
            }
            catch (TokenInfoException tex) {
                await ReplyAsync(tex.Message);
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("TokenInfoPlugin", ex);
                await ReplyAsync("An error has occured during command. Check log for details.");
            }
        }

        [Command("info")]
        [Summary("Shows detailed information about a currency.")]
        public async Task Info([Summary("The ID of the token to display.")] string id = null) {
            using var typing = Context.Channel.EnterTypingState();
            try {
                await ReplyAsync(embed: await TokenInfoCommands.GetInfoAsync(id));
            }
            catch (TokenInfoException tex) {
                await ReplyAsync(tex.Message);
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("TokenInfoPlugin", ex);
                await ReplyAsync("An error has occured during command. Check log for details.");
            }
        }
    }
}

