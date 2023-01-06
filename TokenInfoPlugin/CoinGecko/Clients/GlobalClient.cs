using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.Global;
using CoinGecko.Interfaces;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class GlobalClient : BaseApiClient, IGlobalClient
    {
        public GlobalClient(HttpClient httpClient, JsonSerializerOptions options) : base(httpClient, options)
        {
        }

        public GlobalClient(HttpClient httpClient, JsonSerializerOptions options, string apiKey) : base(httpClient, options, apiKey)
        {
        }

        public async Task<Global> GetGlobal()
        {
            return await GetAsync<Global>(AppendQueryString(GlobalApiEndPoints.Global)).ConfigureAwait(false);
        }

        public async Task<GlobalDeFi> GetGlobalDeFi()
        {
            return await GetAsync<GlobalDeFi>(AppendQueryString(GlobalApiEndPoints.DecentralizedFinanceDeFi)).ConfigureAwait(false);
        }
    }
}