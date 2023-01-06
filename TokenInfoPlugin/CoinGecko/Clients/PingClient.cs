using CoinGecko.Entities.Response;
using CoinGecko.Interfaces;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class PingClient : BaseApiClient, IPingClient
    {
        public PingClient(HttpClient httpClient, JsonSerializerOptions serializerSettings) : base(httpClient, serializerSettings) {
        }

        public PingClient(HttpClient httpClient, JsonSerializerOptions serializerSettings, string apiKey) : base(httpClient, serializerSettings, apiKey) {
        }

        public Task<Ping> GetPingAsync() {
            return GetAsync<Ping>(AppendQueryString("ping"));
        }
    }
}