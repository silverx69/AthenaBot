using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.Coins;
using CoinGecko.Interfaces;
using CoinGecko.Parameters;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class CoinsClient : BaseApiClient, ICoinsClient
    {
        public CoinsClient(HttpClient httpClient, JsonSerializerOptions serializerSettings) : base(httpClient, serializerSettings) {
        }

        public CoinsClient(HttpClient httpClient, JsonSerializerOptions serializerSettings, string apiKey) : base(httpClient, serializerSettings, apiKey) {
        }

        public Task<IReadOnlyList<CoinFullData>> GetAllCoinsData() {
            return GetAllCoinsData(OrderField.GeckoDesc, null, null, "", null);
        }

        public Task<IReadOnlyList<CoinFullData>> GetAllCoinsData(string order, int? perPage, int? page, string localization, bool? sparkline) {
            return GetAsync<IReadOnlyList<CoinFullData>>(AppendQueryString(
                CoinsApiEndPoints.Coins, new Dictionary<string, object> {
                    {"order", order},
                    {"per_page", perPage},
                    {"page",page},
                    {"localization",localization},
                    {"sparkline",sparkline}
                }));
        }

        public Task<IReadOnlyList<CoinList>> GetCoinList() {
            return GetAsync<IReadOnlyList<CoinList>>(AppendQueryString(CoinsApiEndPoints.CoinList));
        }

        public Task<IReadOnlyList<CoinList>> GetCoinList(bool includePlatform) {
            return GetAsync<IReadOnlyList<CoinList>>(AppendQueryString(CoinsApiEndPoints.CoinList,
                new Dictionary<string, object> { 
                    { "include_platform",includePlatform.ToString() }
                }));
        }

        public Task<List<CoinMarkets>> GetCoinMarkets(string vsCurrency) {
            return GetCoinMarkets(vsCurrency, new string[] { }, null, null, null, false, null, null);
        }

        public Task<List<CoinMarkets>> GetCoinMarkets(string vsCurrency, string[] ids, string order, int? perPage,
            int? page, bool sparkline, string priceChangePercentage) {
            return GetCoinMarkets(vsCurrency, ids, order, perPage, page, sparkline, priceChangePercentage, null);
        }

        public Task<List<CoinMarkets>> GetCoinMarkets(string vsCurrency, string[] ids, string order, int? perPage,
            int? page, bool sparkline, string priceChangePercentage, string category) {
            return GetAsync<List<CoinMarkets>>(AppendQueryString(CoinsApiEndPoints.CoinMarkets,
                new Dictionary<string, object> {
                    {"vs_currency", vsCurrency},
                    {"ids", string.Join(",", ids)},
                    {"order",order},
                    {"per_page", perPage},
                    {"page", page},
                    {"sparkline", sparkline},
                    {"price_change_percentage", priceChangePercentage},
                    {"category",category}
                }));
        }

        public Task<CoinFullDataById> GetAllCoinDataWithId(string id) {
            return GetAllCoinDataWithId(id, "true", true, true, true, true, false);
        }

        public Task<CoinFullDataById> GetAllCoinDataWithId(string id, string localization, bool tickers,
            bool marketData, bool communityData, bool developerData, bool sparkline) {
            return GetAsync<CoinFullDataById>(AppendQueryString(
                CoinsApiEndPoints.AllDataByCoinId(id), new Dictionary<string, object> {
                    {"localization", localization},
                    {"tickers", tickers},
                    {"market_data", marketData},
                    {"community_data", communityData},
                    {"developer_data", developerData},
                    {"sparkline", sparkline}
                }));
        }

        public Task<TickerById> GetTickerByCoinId(string id) {
            return GetTickerByCoinId(id, new[] { "" }, null);
        }

        public Task<TickerById> GetTickerByCoinId(string id, int? page) {
            return GetTickerByCoinId(id, new[] { "" }, page);
        }

        public Task<TickerById> GetTickerByCoinId(string id, string[] exchangeIds, int? page) {
            return GetTickerByCoinId(id, exchangeIds, page, "", OrderField.TrustScoreDesc, false);
        }

        public Task<TickerById> GetTickerByCoinId(string id, string[] exchangeIds, int? page, string includeExchangeLogo, string order, bool depth) {
            return GetAsync<TickerById>(AppendQueryString(
                CoinsApiEndPoints.TickerByCoinId(id), new Dictionary<string, object> {
                    {"page", page},
                    {"exchange_ids",string.Join(",",exchangeIds)},
                    {"include_exchange_logo",includeExchangeLogo},
                    {"order",order},
                    {"depth",depth.ToString()}
                }));
        }

        public Task<CoinFullData> GetHistoryByCoinId(string id, string date, string localization) {
            return GetAsync<CoinFullData>(AppendQueryString(
                CoinsApiEndPoints.HistoryByCoinId(id), new Dictionary<string, object> {
                    {"date",date},
                    {"localization",localization}
                }));
        }

        public Task<MarketChartById> GetMarketChartsByCoinId(string id, string vsCurrency, string days) {
            return GetMarketChartsByCoinId(id, vsCurrency, days, "");
        }

        public Task<MarketChartById> GetMarketChartsByCoinId(string id, string vsCurrency, string days, string interval) {
            return GetAsync<MarketChartById>(AppendQueryString(
                CoinsApiEndPoints.MarketChartByCoinId(id),
                new Dictionary<string, object> {
                    {"vs_currency", string.Join(",",vsCurrency)},
                    {"days", days},
                    {"interval",interval}
                }));
        }

        public Task<MarketChartById> GetMarketChartRangeByCoinId(string id, string vsCurrency, string @from, string to) {
            return GetAsync<MarketChartById>(AppendQueryString(
                CoinsApiEndPoints.MarketChartRangeByCoinId(id), new Dictionary<string, object> {
                    {"vs_currency", string.Join(",", vsCurrency)},
                    {"from",from},
                    {"to",to}
                }));
        }

        public Task<IReadOnlyList<IReadOnlyList<object>>> GetCoinOhlc(string id, string vsCurrency, int days) {
            return GetAsync<IReadOnlyList<IReadOnlyList<object>>>(AppendQueryString(
                CoinsApiEndPoints.CoinOhlc(id), new Dictionary<string, object> {
                    {"vs_currency", vsCurrency},
                    {"days", days}
                }));
        }
    }
}