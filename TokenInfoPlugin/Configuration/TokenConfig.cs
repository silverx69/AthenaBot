using AthenaBot;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin.Configuration
{
    public class TokenConfig : ModelBase
    {
        [JsonPropertyName("coingecko_id")]
        public string CoinGeckoId { get; set; }

        [JsonPropertyName("decimals")]
        public int Decimals { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }

        [JsonPropertyName("dev_wallets")]
        public List<string> DevWallets { get; set; }

        [JsonPropertyName("burn_wallets")]
        public List<string> BurnWallets { get; set; }

        public TokenConfig() {
            Enabled = true;
            DevWallets = new List<string>();
            BurnWallets = new List<string>();
        }

        public TokenConfig(string coinGeckoId)
            : this() {
            CoinGeckoId = coinGeckoId;
        }
    }
}
