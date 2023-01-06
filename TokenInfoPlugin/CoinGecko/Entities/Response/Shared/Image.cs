using System;
using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Shared
{
    public class Image
    {
        [JsonPropertyName("thumb")]
        public Uri Thumb { get; set; }

        [JsonPropertyName("small")]
        public Uri Small { get; set; }

        [JsonPropertyName("large")]
        public Uri Large { get; set; }
    }
}