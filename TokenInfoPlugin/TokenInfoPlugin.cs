using AthenaBot;
using AthenaBot.Plugins;
using BscScan.NetCore.Configuration;
using BscScan.NetCore.Services;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using Newtonsoft.Json;
using TokenInfoPlugin.Configuration;

namespace TokenInfoPlugin
{
    public class TokenInfoPlugin : DiscordBotPlugin
    {
        Timer updateTimer = null;
        Dictionary<string, TokenInfo> recentInfos;
        static readonly JsonSerializerSettings jsonSettings = new();

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

            recentInfos = new Dictionary<string, TokenInfo>();
            updateTimer = new Timer(
                OnTimerElapsed,
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(2));
        }

        private async void OnTimerElapsed(object state) {

            var nicks = Config.Servers.Where(s => s.PriceNickname);

            foreach (var server in nicks) {
                var guild = Bot.Client.GetGuild(server.Id);
                if (guild != null) {
                    var def = server.Tokens.Find(s => s.Default);
                    if (def != null) {
                        try {
                            var token = await GetTokenInfo(server, def, 1);
                            string name = $"${token.Price} USD";
                            if (guild.CurrentUser.Nickname != name)
                                await guild.CurrentUser.ModifyAsync(s => s.Nickname = name);
                        }
                        catch { }
                    }
                }
            }
        }

        public override void OnPluginKilled() {
            updateTimer.Change(-1, -1);
            updateTimer.Dispose();
            updateTimer = null;
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
            recentInfos.Clear();
            recentInfos = null;
        }

        internal async Task<TokenInfo> GetTokenInfo(ulong guildId, string id) {
            bool newConfig = false;
            TokenConfig tconfig = null;

            var config = Config.Servers.Find(s => s.Id == guildId);
            if (config == null) {
                config = new ServerConfig();
                Config.Servers.Add(config);
            }

            if (config.AnyToken && !string.IsNullOrWhiteSpace(id))
                tconfig = config.Tokens.Find(s => s.CoinGeckoId == id || s.Aliases.Contains(id));
            else {
                tconfig = config.Tokens.Find(s => s.Default);
                if (tconfig == null)
                    throw new TokenInfoException("No default token has been configured.");
            }

            if (tconfig == null) {
                newConfig = true;
                tconfig = new TokenConfig(id);
            }

            var recent = await GetTokenInfo(config, tconfig);

            if (newConfig) {
                config.Tokens.Add(tconfig);
                await Persistence.SaveModelAsync(
                    Config,
                    Path.Combine(Directory, "config.json"));
            }

            return recent;
        }

        internal async Task<TokenInfo> GetTokenInfo(ServerConfig config, TokenConfig tconfig, int delay = 5) {
            DateTime now = DateTime.Now;

            if (!recentInfos.TryGetValue(tconfig.CoinGeckoId, out TokenInfo recent))
                recent = new TokenInfo(now);

            if (now.Subtract(recent.LastUpdate).TotalMinutes >= delay) {
                using var client = new HttpClient();
                var ping = new PingClient(client, jsonSettings);

                if (string.IsNullOrEmpty((await ping.GetPingAsync()).GeckoSays))
                    throw new TokenInfoException("Unable to contact CoinGecko");

                CoinFullDataById coin = null;
                var coins = new CoinsClient(client, jsonSettings);

                try {
                    coin = await coins.GetAllCoinDataWithId(tconfig.CoinGeckoId);
                }
                catch (HttpRequestException hex) {
                    if (hex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new TokenInfoException(string.Format("The token id \"{0}\" was not found.", tconfig.CoinGeckoId));
                    else throw;
                }

                recentInfos[tconfig.CoinGeckoId] = recent;
                recent.LastUpdate = now;
                recent.Name = coin.Name;
                recent.MarketCap = coin.MarketData?.MarketCap["usd"] ?? 0M;
                recent.TotalSupply = coin.MarketData?.TotalSupply ?? 0M;
                if (decimal.TryParse(coin.MarketData?.CirculatingSupply ?? "0", out decimal supply))
                    recent.CirculatingSupply = supply;
                recent.Price = coin.MarketData?.CurrentPrice["usd"] ?? 0M;
                recent.Contracts = coin.Platforms;
                recent.Homepage = coin.Links?.Homepage.FirstOrDefault() ?? string.Empty;
                recent.Thumbnail = coin.Image.Thumb?.ToString() ?? string.Empty;

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
                    recent.Treasury = await GetSumFromWallets(service, contract, tconfig, tconfig.DevWallets);
                    recent.Burned = await GetSumFromWallets(service, contract, tconfig, tconfig.BurnWallets);
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