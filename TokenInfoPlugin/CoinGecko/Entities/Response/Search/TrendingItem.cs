using System.Text.Json.Serialization;

namespace CoinGecko.Entities.Response.Search
{
    public class TrendingItemWrapper
    {
        [JsonPropertyName("item")]
        public TrendingItem TrendingItem { get; set; }
    }

    public class TrendingItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("coin_id")]
        public int CoinId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("market_cap_rank")]
        public int MarketCapRank { get; set; }

        [JsonPropertyName("thumb")]
        public string Thumb { get; set; }

        [JsonPropertyName("small")]
        public string Small { get; set; }

        [JsonPropertyName("large")]
        public string Large { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("price_btc")]
        public double PriceBtc { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }
    }
}
