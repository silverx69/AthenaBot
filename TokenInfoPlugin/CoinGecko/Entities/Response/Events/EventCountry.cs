using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Events
{
    public class EventCountry
    {
        [JsonPropertyName("data")]
        public EventCountryData[] Data { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }
    }

    public class EventCountryData
    {
        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}