using AthenaBot.Commands;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TranslatePlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class MessageCommandModule : DiscordBotSilentInteractionModule
    {
        [MessageCommand("Translate")]
        public async Task Translate(IMessage imsg) {

            if (imsg is not SocketMessage message)
                return;

            var result = await TranslateCommands.TranslateAsync(Context.Guild.Id, message.CleanContent);
            if (result == null)
                await FollowupAsync("Nothing to translate.");
            else
                await FollowupAsync(embed: result);
        }
    }
}
