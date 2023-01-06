using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.Search;
using CoinGecko.Interfaces;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class SearchClient : BaseApiClient, ISearchClient
    {
        public SearchClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerSetting) : base(httpClient, jsonSerializerSetting)
        {
        }

        public SearchClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerSetting, string apiKey) : base(httpClient, jsonSerializerSetting, apiKey)
        {
        }

        public async Task<TrendingList> GetSearchTrending()
        {
            return await GetAsync<TrendingList>(
                 AppendQueryString(SearchApiEndpoints.SearchTrending))
                .ConfigureAwait(false);
        }
    }
}