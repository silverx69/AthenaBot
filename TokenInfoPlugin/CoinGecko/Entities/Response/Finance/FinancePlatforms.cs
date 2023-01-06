using System;
using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Finance
{
    public class FinancePlatforms
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("facts")]
        public string Facts { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("centralized")]
        public bool Centralized { get; set; }

        [JsonPropertyName("website_url")]
        public Uri WebsiteUrl { get; set; }
    }
}