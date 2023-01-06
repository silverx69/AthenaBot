using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Exchanges
{
    public class ExchangesList
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}