using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Events
{
    public partial class Events
    {
        [JsonPropertyName("data")]
        public EventData[] Data { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }

        [JsonPropertyName("page")]
        public long? Page { get; set; }
    }
}