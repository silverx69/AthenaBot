using AthenaBot.Interactions;
using Discord;
using Discord.Interactions;

namespace GoogleTranslatePlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class MessageCommandModule : AthenaInteractionModule
    {
        [MessageCommand("Translate")]
        public async Task Translate(IMessage imsg) {

            await DeferAsync(ephemeral: true);

            var result = await TranslateCommands.TranslateAsync(Context.Guild.Id, imsg.CleanContent);
            if (result == null)
                await FollowupAsync("Nothing to translate.");
            else
                await FollowupAsync(embed: result);
        }
    }
}
