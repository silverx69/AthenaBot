using AthenaBot;
using Discord;
using System.Text;

namespace TokenInfoPlugin.Commands
{
    static class TokenInfoCommands
    {
        public static async Task<Embed> GetPriceAsync(ulong guildId, string id) {
            var recent = await TokenInfoPlugin.Self.GetTokenInfo(id, guildId);

            var eb = new EmbedBuilder() {
                Title = string.Format("#{0} | {1}", recent.Rank, recent.Name),
                Url = recent.Homepage,
                ThumbnailUrl = recent.Thumbnail,
                Description = string.Format(
                    "{0} is currently at ${1} USD.",
                    recent.Name,
                    recent.Price)
            };

            GetEmbedFooter(eb, recent);
            return eb.Build();
        }

        public static async Task<Embed> GetInfoAsync(ulong guildId, string id) {
            var recent = await TokenInfoPlugin.Self.GetTokenInfo(id, guildId);

            var eb = new EmbedBuilder() {
                Title = string.Format("#{0} | {1}", recent.Rank, recent.Name),
                Url = recent.Homepage,
                ThumbnailUrl = recent.Thumbnail
            };

            var sb = new StringBuilder();
            //build Market field
            sb.AppendLine("__Price:__");
            sb.AppendLine(string.Format("${0}", recent.Price));
            sb.AppendLine("__Volume 24h:__");
            sb.AppendLine(recent.Volume24h.ToString("C"));
            if (recent.MarketCapLive <= 0M) {
                sb.AppendLine("__Market Cap:__");
                sb.AppendLine(recent.MarketCap.ToString("C"));
            }
            else {
                sb.AppendLine("__Market Cap (Live):__");
                sb.AppendLine(recent.MarketCapLive.ToString("C"));
            }
            //add Market field
            eb.AddField("**Market (USD)**", sb.ToString());
            sb.Clear();
            //build Trends field
            sb.AppendLine(GetPercentageFormat("1h", recent.PriceChange1h));
            sb.AppendLine(GetPercentageFormat("24h", recent.PriceChange24h));
            sb.AppendLine(GetPercentageFormat("7d", recent.PriceChange7d));
            sb.AppendLine(GetPercentageFormat("14d", recent.PriceChange14d));
            sb.AppendLine(GetPercentageFormat("30d", recent.PriceChange30d));
            sb.AppendLine(GetPercentageFormat("60d", recent.PriceChange60d));
            //add Trends field
            eb.AddField("**Trends**", sb.ToString());
            sb.Clear();
            //build Supply field
            sb.AppendLine("__Circulating:__");
            sb.AppendLine(recent.CirculatingSupply.ToString("N"));
            if (recent.Burned > 0M) {
                sb.AppendLine("__Burned:__");
                sb.AppendLine(recent.Burned.ToString("N"));
            }
            sb.AppendLine("__Total:__");
            sb.Append(recent.TotalSupply.ToString("N"));
            //add supply field
            eb.AddField("**Supply**", sb.ToString());

            GetEmbedFooter(eb, recent);
            return eb.Build();
        }

        private static void GetEmbedFooter(EmbedBuilder eb, TokenInfo recent) {
            eb.WithColor(recent.Color);
            eb.WithFooter("Last Updated");
            eb.WithTimestamp(recent.LastUpdate.ToUtcOffset());
        }

        private static string GetPercentageFormat(string range, double value) {
            return string.Format(
                "__{0} Change:__ {1}% {2}",
                range,
                value.ToString("N"),
                GetUpDownEmoji(value));
        }

        private static string GetUpDownEmoji(double value) {
            if (value > 0d)
                return "↗️";
            if (value < 0d)
                return "↘️";
            return "➡️";
        }
    }
}
