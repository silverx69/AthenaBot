using Discord;
using System.Text;

namespace TokenInfoPlugin.Commands
{
    static class TokenInfoCommands
    {
        public static async Task<Embed> GetPriceAsync(ulong guildId, string id) {

            var recent = await TokenInfoPlugin.Self.GetTokenInfo(guildId, id);

            var eb = new EmbedBuilder() {
                Title = recent.Name,
                Url = recent.Homepage,
                ThumbnailUrl = recent.Thumbnail,
                Description = string.Format(
                    "{0} is currently at ${1} USD.",
                    recent.Name,
                    recent.Price)
            };

            eb.WithCurrentTimestamp();
            return eb.Build();
        }

        public static async Task<Embed> GetInfoAsync(ulong guildId, string id) {

            var recent = await TokenInfoPlugin.Self.GetTokenInfo(guildId, id);

            var eb = new EmbedBuilder() {
                Title = recent.Name,
                Url = recent.Homepage,
                ThumbnailUrl = recent.Thumbnail
            };

            var sb = new StringBuilder();
            //build Market field
            sb.AppendFormat("**Price:** ${0}", recent.Price);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("**Market Cap:** {0}", recent.MarketCap.ToString("C"));
            sb.AppendLine();
            //add Market field
            eb.AddField("__Market__", sb.ToString());
            sb.Clear();
            //build Supply field
            sb.AppendFormat("**Circulating:** {0}", recent.CirculatingSupply.ToString("N"));
            sb.AppendLine();
            sb.AppendLine();
            if (recent.Burned > 0M) {
                sb.AppendFormat("**Burned:** {0}", recent.Burned.ToString("N"));
                sb.AppendLine();
                sb.AppendLine();
            }
            sb.AppendFormat("**Total:** {0}", recent.TotalSupply.ToString("N"));
            //add supply field
            eb.AddField("__Supply__", sb.ToString());

            //use desc instead of fields?
            //eb.WithDescription(sb.ToString());

            eb.WithCurrentTimestamp();
            return eb.Build();
        }
    }
}
