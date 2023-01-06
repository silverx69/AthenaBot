using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.ExchangeRates;
using CoinGecko.Interfaces;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class ExchangeRatesClient : BaseApiClient, IExchangeRatesClient
    {
        public ExchangeRatesClient(HttpClient httpClient, JsonSerializerOptions serializerSettings) : base(httpClient, serializerSettings)
        {
        }

        public ExchangeRatesClient(HttpClient httpClient, JsonSerializerOptions serializerSettings, string apiKey) : base(httpClient, serializerSettings, apiKey)
        {
        }

        public async Task<ExchangeRates> GetExchangeRates()
        {
            return await GetAsync<ExchangeRates>(
                AppendQueryString(ExchangeRatesApiEndPoints.ExchangeRate)).ConfigureAwait(false);
        }
    }
}