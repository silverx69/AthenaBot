using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Shared
{
    public class PublicInterestStats
    {
        [JsonPropertyName("alexa_rank")] public long? AlexaRank { get; set; }

        [JsonPropertyName("bing_matches")] public long? BingMatches { get; set; }
    }
}