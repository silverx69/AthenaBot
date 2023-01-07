using AthenaBot;
using System.Text.Json.Serialization;

namespace TokenInfoPlugin
{
    public class TokenInfo : ModelBase
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; internal set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("marketCap")]
        public decimal MarketCap { get; set; }

        [JsonPropertyName("totalSupply")]
        public decimal TotalSupply { get; set; }

        [JsonPropertyName("circulatingSupply")]
        public decimal CirculatingSupply { get; set; }

        [JsonPropertyName("treasury")]
        public decimal Treasury { get; set; }

        [JsonPropertyName("burned")]
        public decimal Burned { get; set; }

        [JsonPropertyName("contracts")]
        public Dictionary<string, string> Contracts { get; set; }

        [JsonIgnore]
        public DateTime LastUpdate { get; set; }

        public TokenInfo()
            : this(DateTime.Now) { }

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
