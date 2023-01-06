using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Coins
{
    public class CoinList
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("platforms")]
        public Dictionary<string,string> Platforms { get; set; }
    }
}