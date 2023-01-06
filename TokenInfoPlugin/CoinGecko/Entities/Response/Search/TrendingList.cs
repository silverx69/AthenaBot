using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Search
{
    public class TrendingList
    {
        [JsonPropertyName("coins")]
        public TrendingItemWrapper[] TrendingItems { get; set; }

        // It's unknown what kind of data contains "exchanges" because
        // CoinGecko API return empty list, so using object[] declaration
        [JsonPropertyName("exchanges")]
        public object[] Exchanges { get; set; }
    }
}
