using AthenaBot;
using Discord;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin
{
    public class TokenInfo : ModelBase
    {
        public string Name { get; set; }

        public string Homepage { get; set; }

        public string Thumbnail { get; internal set; }

        public Color Color { get; set; }

        public long Rank { get; set; }

        public decimal Price { get; set; }

        public decimal Volume24h { get; set; }

        public decimal MarketCap { get; set; }

        public decimal MarketCapLive { get; set; }

        public decimal TotalSupply { get; set; }

        public decimal CirculatingSupply { get; set; }

        public decimal Treasury { get; set; }

        public decimal Burned { get; set; }

        public double PriceChange1h { get; set; }

        public double PriceChange24h { get; set; }

        public double PriceChange7d { get; set; }

        public double PriceChange14d { get; set; }

        public double PriceChange30d { get; set; }

        public double PriceChange60d { get; set; }

        public Dictionary<string, string> Contracts { get; set; }

        [JsonIgnore]
        public DateTime LastUpdate { get; set; }

        public TokenInfo()
            : this(DateTime.UtcNow) { }

        public TokenInfo(DateTime update) {
            LastUpdate = update.Subtract(TimeSpan.FromMinutes(10));
            Contracts = new Dictionary<string, string>();
        }
    }

    public sealed class TokenInfoException : Exception
    {
        public TokenInfoException(string message)
            : base(message) { }
    }
}
