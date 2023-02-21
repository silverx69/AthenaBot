using AthenaBot.Commands;
using Discord.Interactions;

namespace TranslatePlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class SlashCommandModule : DiscordBotSilentInteractionModule
    {
        [SlashCommand("translate", "Translates a specified body of text into another language.")]
        public async Task Translate(
            [Summary(description: "The text to be translated.")]
            string text,
            [Summary(description: "The language to translate to.")]
            string to = null,
            [Summary(description: "The language to translate from.")]
            string from = null) {

            var result = await TranslateCommands.TranslateAsync(Context.Guild.Id, text, to, from);
            if (result == null)
                await FollowupAsync("Nothing to translate.");
            else
                await FollowupAsync(embed: result);
        }
    }
}
