using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Events
{
    public partial class EventTypes
    {
        [JsonPropertyName("data")]
        public string[] Data { get; set; }

        [JsonPropertyName("count")]
        public long? Count { get; set; }
    }
}