using Discord;

namespace AthenaBot
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder WithLastUpdated(this EmbedBuilder eb, DateTimeOffset time) {
            return eb.WithFooter("Last Updated").WithTimestamp(time);
        }
    }
}
