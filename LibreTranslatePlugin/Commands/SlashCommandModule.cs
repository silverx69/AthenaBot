using AthenaBot.Interactions;
using Discord;
using Discord.Interactions;

namespace LibreTranslatePlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class SlashCommandModule : AthenaInteractionModule
    {
        [SlashCommand("translate", "Translates a specified body of text into another language.")]
        public async Task Translate(
            [Summary(description: "The text to be translated.")]
            string text,
            [Summary(description: "The language to translate to.")]
            string to = null,
            [Summary(description: "The language to translate from.")]
            string from = "auto") {

            await DeferAsync(ephemeral: true);

            to ??= Context.Interaction.UserLocale;

            var result = await LibreTranslatePlugin.Self.TranslateAsync(text, to, from);
            if (result == null)
                await FollowupAsync("Nothing to translate.");
            else {
                var embed = new EmbedBuilder()
                    .AddField("Source", text)
                    .AddField("Result", result.TranslatedText)
                    .WithFooter(string.Format(
                        "{0} -> {1}",
                        LibreTranslatePlugin.FromLanguageCode(result.SourceLanguage),
                        LibreTranslatePlugin.FromLanguageCode(result.TranslatedLanguage)))
                    .Build();
                await FollowupAsync(embed: embed);
            }
        }
    }
}
