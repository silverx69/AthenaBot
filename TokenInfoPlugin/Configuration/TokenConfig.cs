using AthenaBot;
using Discord;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin.Configuration
{
    public class TokenConfig : ModelBase
    {
        [JsonPropertyName("coingecko_id")]
        public string CoinGeckoId { get; set; }

        public int Decimals { get; set; }

        public bool Enabled { get; set; }

        public bool Default { get; set; }

        public Color Color { get; set; }

        public List<string> Aliases { get; set; }

        [JsonPropertyName("dev_wallets")]
        public List<string> DevWallets { get; set; }

        [JsonPropertyName("burn_wallets")]
        public List<string> BurnWallets { get; set; }

        public TrendingConfig Trending { get; set; }

        public TokenConfig() {
            Enabled = true;
            Color = Color.Default;
            Aliases = new List<string>();
            DevWallets = new List<string>();
            BurnWallets = new List<string>();
            Trending = new TrendingConfig();
        }

        public TokenConfig(string coinGeckoId)
            : this() {
            CoinGeckoId = coinGeckoId;
        }
    }
}
