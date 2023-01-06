using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Derivatives
{
    public class DerivativesExchangesList
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}