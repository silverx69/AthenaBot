using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.ExchangeRates
{
    public class ExchangeRates
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, Rate> Rates { get; set; }
    }

    public class Rate
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("unit")]
        public string Unit { get; set; }

        [JsonPropertyName("value")]
        public decimal? Value { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}