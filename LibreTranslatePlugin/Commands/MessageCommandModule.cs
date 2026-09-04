using AthenaBot.Interactions;
using Discord;
using Discord.Interactions;

namespace LibreTranslatePlugin.Commands
{
    [RequireContext(ContextType.Guild)]
    public class MessageCommandModule : AthenaInteractionModule
    {
        [MessageCommand("Translate")]
        public async Task Translate(IMessage imsg) {
            await DeferAsync(ephemeral: true);

            string to = LibreTranslatePlugin.ToLanguageCode(Context.Interaction.UserLocale);
            var result = await LibreTranslatePlugin.Self.TranslateAsync(imsg.CleanContent, to);

            if (result == null)
                await FollowupAsync("Nothing to translate.");
            else {
                var embed = new EmbedBuilder()
                    .WithDescription(result.TranslatedText)
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
