using AthenaBot;
using AthenaBot.Commands;
using Discord.Commands;

namespace TokenInfoPlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class TextCommandModule : AthenaCommandModule
    {
        [Command("price")]
        [Summary("Displays a currency's current price information.")]
        public async Task Price([Summary("The ID of the token to display."), Remainder] string id = null) {
            using var typing = Context.Channel.EnterTypingState();
            try {
                await ReplyAsync(embed: await TokenInfoCommands.GetPriceAsync(Context.Guild.Id, id));
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

