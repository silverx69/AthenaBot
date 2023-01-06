using AthenaBot;
using AthenaBot.Plugins;
using BscScan.NetCore.Configuration;
using System.Text.Json.Serialization;
using System.Text.Json;
using TokenInfoPlugin.Configuration;
using BscScan.NetCore.Services;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;

namespace TokenInfoPlugin
{
    public class TokenInfoPlugin : DiscordBotPlugin
    {
        JsonSerializerOptions jsonOptions;
        Dictionary<string, TokenInfo> recentInfos;

        const string BscScanApi = "https://api.bscscan.com/api";

        internal static PluginConfig Config {
            get;
            private set;
        }

        internal static TokenInfoPlugin Self {
            get;
            private set;
        }

        public TokenInfoPlugin() { Self = this; }

        public override void OnPluginLoaded() {
            string file = Path.Combine(Directory, "config.json");

            Config = Persistence.LoadModel<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.SaveModel(Config, file);

            jsonOptions = new JsonSerializerOptions(Json.Options) {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };

            recentInfos = new Dictionary<string, TokenInfo>();
        }

        public override void OnPluginKilled() {
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
            jsonOptions = null;
            recentInfos.Clear();
            recentInfos = null;
        }

        internal async Task<TokenInfo> GetTokenInfo(string id) {
            bool newConfig = false;
            TokenConfig config = null;

            if (Config.AnyToken && !string.IsNullOrWhiteSpace(id))
                config = Config.Tokens.Find(s => s.CoinGeckoId == id);
            else {
                config = Config.Tokens.Find(s => s.Default);
                if (config == null)
                    throw new TokenInfoException("No default token has been configured.");
            }

            if (config == null) {
                newConfig = true;
                config = new TokenConfig(id);
            }

            DateTime now = DateTime.Now;

            if (!recentInfos.TryGetValue(config.CoinGeckoId, out TokenInfo recent))
                recent = new TokenInfo(now);

            if (now.Subtract(recent.LastUpdate).TotalMinutes >= 5) {
                using var client = new HttpClient();
                var ping = new PingClient(client, jsonOptions);

                if (string.IsNullOrEmpty((await ping.GetPingAsync()).GeckoSays))
                    throw new TokenInfoException("Unable to contact CoinGecko");

                CoinFullDataById coin = null;
                var coins = new CoinsClient(client, jsonOptions);

                try {
                    coin = await coins.GetAllCoinDataWithId(config.CoinGeckoId);
                }
                catch(HttpRequestException hex) {
                    if (hex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new TokenInfoException(string.Format("The token id \"{0}\" was not found.", config.CoinGeckoId));
                    else throw;
                }

                recentInfos[config.CoinGeckoId] = recent;
                recent.LastUpdate = now;
                recent.Name = coin.Name;
                recent.MarketCap = coin.MarketData?.MarketCap["usd"] ?? 0M;
                recent.TotalSupply = coin.MarketData?.TotalSupply ?? 0M;
                recent.CirculatingSupply = coin.MarketData?.CirculatingSupply ?? 0M;
                recent.Price = coin.MarketData?.CurrentPrice["usd"] ?? 0M;
                recent.Contracts = coin.Platforms;
                recent.Homepage = coin.Links?.Homepage.FirstOrDefault() ?? string.Empty;
                recent.Thumbnail = coin.Image.Thumb?.ToString() ?? string.Empty;

                if (newConfig) {
                    Config.Tokens.Add(config);
                    await Persistence.SaveModelAsync(
                        Config,
                        Path.Combine(Directory, "config.json"));
                }

                if (!string.IsNullOrEmpty(Config.BscScanApiKey) &&
                    recent.Contracts.TryGetValue("binance-smart-chain", out string contract)) {

                    using var bscClient = new HttpClient();
                    bscClient.BaseAddress = new Uri(BscScanApi);

                    var service = new BscScanTokensService(bscClient, new BscScanConfiguration() {
                        BscScanOptions = new BscScanOptions() {
                            Token = Config.BscScanApiKey,
                            Uri = BscScanApi
                        }
                    });
                    recent.Treasury = await GetSumFromWallets(service, contract, config, config.DevWallets);
                    recent.Burned = await GetSumFromWallets(service, contract, config, config.BurnWallets);
                    recent.CirculatingSupply = recent.TotalSupply - recent.Treasury - recent.Burned;
                }
            }
            return recent;
        }

        private static async Task<decimal> GetSumFromWallets(BscScanTokensService service, string contract, TokenConfig config, List<string> wallets) {
            decimal sum = 0M;
            for (int i = 0; i < wallets.Count; i++) {
                string wallet = wallets[i];

                var result = await service.GetBep20TokenAccountBalanceByContractAddressAsync(contract, wallet);

                string balance = result.Result;
                if (string.IsNullOrEmpty(balance)) continue;

                if (config.Decimals > 0)
                    balance = balance[..^config.Decimals] + "." + balance[^config.Decimals..];

                if (decimal.TryParse(balance, out decimal value))
                    sum += value;

                if (i < config.BurnWallets.Count - 1)
                    await Task.Delay(500);
            }
            return sum;
        }
    }
}