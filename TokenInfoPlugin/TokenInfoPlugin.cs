using AthenaBot;
using AthenaBot.Plugins;
using BscScan.NetCore.Configuration;
using BscScan.NetCore.Services;
using CoinGecko.Clients;
using CoinGecko.Entities.Response.Coins;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using TokenInfoPlugin.Configuration;

namespace TokenInfoPlugin
{
    public class TokenInfoPlugin : DiscordBotPlugin
    {
        Timer updateTimer = null;
        DateTime lastNicknames;

        volatile bool isUpdating = false;
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

        public TokenInfoPlugin() {
            Self = this;
        }

        public override void OnPluginLoaded() {
            string file = Path.Combine(Directory, "config.json");

            Config = Persistence.LoadModel<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.SaveModel(Config, file);

            if (DateTime.MinValue.Equals(lastNicknames))
                lastNicknames = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(10));

            if (DateTime.MinValue.Equals(Config.LastTrending))
                Config.LastTrending = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4));

            recentInfos = new Dictionary<string, TokenInfo>();
            updateTimer = new Timer(
                OnTimerElapsed,
                null,
                TimeSpan.FromSeconds(20),
                TimeSpan.FromMinutes(1));
        }

        public override void OnPluginKilled() {
            updateTimer.Change(-1, -1);
            updateTimer.Dispose();
            updateTimer = null;
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
            recentInfos.Clear();
            recentInfos = null;
        }

        private async void OnTimerElapsed(object state) {
            if (isUpdating || !Bot.IsReady)
                return;

            isUpdating = true;
            try {
                if (await CheckNicknames())
                    return;

                await CheckTrending();
            }
            catch (Exception ex) {
                await Logging.ErrorAsync("TokenInfoPlugin", ex);
            }
            finally { isUpdating = false; }
        }

        private async Task<bool> CheckNicknames() {
            DateTime now = DateTime.UtcNow;

            if (now.Subtract(lastNicknames).TotalMinutes < 5)
                return false;

            lastNicknames = now;

            var nicks = Config.Servers
                .Where(s => s.PriceNickname)
                .ToList();

            for (int i = 0; i < nicks.Count; i++) {
                var server = nicks[i];

                var guild = Bot.Client.GetGuild(server.Id);
                if (guild == null) continue;

                var def = server.Tokens.Find(s => s.Default);
                if (def == null) continue;

                var token = await GetTokenInfo(def, 4.9d);
                string name = string.Format("${0}", token.Price);

                if (string.Compare(guild.CurrentUser.Nickname, name) != 0)
                    await guild.CurrentUser.ModifyAsync(s => s.Nickname = name);

                if (i < (nicks.Count - 1))
                    await Task.Delay(500);
            }

            return true;
        }

        private async Task CheckTrending() {
            DateTime now = DateTime.UtcNow;

            if (now.Subtract(Config.LastTrending).TotalHours < 4d)
                return;

            Config.LastTrending = now;

            var trending = await GetTrendingSearches();
            if (trending == null) return;

            foreach (var server in Config.Servers) {
                var guild = Bot.Client.GetGuild(server.Id);
                if (guild == null) continue;
                foreach (var token in server.Tokens) {
                    if (!token.Trending.Enabled)
                        continue;
                    //is trending now?
                    var trend = trending.Find(s => s == token.CoinGeckoId);
                    if (trend == null) continue;

                    //was already trending?
                    var prevTrend = Config.RecentTrending.Find(s => s == token.CoinGeckoId);
                    if (prevTrend != null) continue;

                    var embed = await GetTrendingEmbed(token, guild.Id);

                    foreach (ulong id in token.Trending.Channels) {
                        if (guild.Channels.Find(s => s.Id == id) is not SocketTextChannel channel)
                            continue;
                        await channel.SendMessageAsync(embed: embed);
                    }
                }
            }

            Config.RecentTrending = trending;
        }

        private async Task<Embed> GetTrendingEmbed(TokenConfig token, ulong guildId) {
            var recent = await GetTokenInfo(token.CoinGeckoId, guildId);

            var eb = new EmbedBuilder() {
                Title = string.Format("#{0} | {1}", recent.Rank, recent.Name),
                Url = recent.Homepage,
                ThumbnailUrl = recent.Thumbnail,
                Description = string.Format("{0} is currently trending on CoinGecko!", recent.Name)
            };

            eb.WithColor(recent.Color);
            eb.WithCurrentTimestamp();
            return eb.Build();
        }

        internal async Task<TokenInfo> GetTokenInfo(string id, ulong guildId = 0L) {
            TokenConfig tconfig = null;

            if (guildId != 0) {
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
                    tconfig = new TokenConfig(id);
                    config.Tokens.Add(tconfig);
                }
            }
            else if (string.IsNullOrWhiteSpace(id))
                throw new TokenInfoException("Token ID was not supplied.");

            else tconfig = new TokenConfig(id);

            return await GetTokenInfo(tconfig);
        }

        internal async Task<TokenInfo> GetTokenInfo(TokenConfig tconfig, double delay = 5d) {
            DateTime now = DateTime.UtcNow;

            if (!recentInfos.TryGetValue(tconfig.CoinGeckoId, out TokenInfo recent))
                recent = new TokenInfo(now);

            if (now.Subtract(recent.LastUpdate).TotalMinutes < delay)
                return recent;

            recentInfos[tconfig.CoinGeckoId] = recent;
            recent.LastUpdate = now;

            CoinFullDataById coin = null;
            using (var client = new HttpClient()) {
                try {
                    var pings = new PingClient(client, jsonSettings);
                    var ping = await pings.GetPingAsync();

                    if (string.IsNullOrEmpty(ping.GeckoSays))
                        throw new TokenInfoException("Unable to contact CoinGecko");

                    await Task.Delay(500);

                    var coins = new CoinsClient(client, jsonSettings);
                    coin = await coins.GetAllCoinDataWithId(tconfig.CoinGeckoId);
                }
                catch (HttpRequestException hex) {
                    if (hex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new TokenInfoException(string.Format("The token id \"{0}\" was not found.", tconfig.CoinGeckoId));
                    else throw;
                }
            }
            recent.Color = tconfig.Color;
            recent.Name = coin.Name;
            recent.Contracts = coin.Platforms;
            recent.Homepage = coin.Links?.Homepage?.FirstOrDefault() ?? string.Empty;
            recent.Thumbnail = coin.Image?.Large?.ToString() ?? string.Empty;

            if (coin.MarketData != null) {
                recent.Rank = coin.MarketCapRank ?? 0L;
                recent.Price = coin.MarketData.CurrentPrice["usd"] ?? 0M;
                recent.Volume24h = coin.MarketData.TotalVolume["usd"] ?? 0M;
                recent.TotalSupply = coin.MarketData.TotalSupply ?? 0M;
                if (decimal.TryParse(coin.MarketData.CirculatingSupply ?? "0", out decimal supply))
                    recent.CirculatingSupply = supply;
                recent.MarketCap = coin.MarketData.MarketCap["usd"] ?? 0M;
                recent.PriceChange1h = coin.MarketData.PriceChangePercentage1HInCurrency["usd"];
                recent.PriceChange24h = coin.MarketData.PriceChangePercentage24HInCurrency["usd"];
                recent.PriceChange7d = coin.MarketData.PriceChangePercentage7DInCurrency["usd"];
                recent.PriceChange14d = coin.MarketData.PriceChangePercentage14DInCurrency["usd"];
                recent.PriceChange30d = coin.MarketData.PriceChangePercentage30DInCurrency["usd"];
                recent.PriceChange60d = coin.MarketData.PriceChangePercentage60DInCurrency["usd"];
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

                recent.Treasury = await GetSumFromWallets(service, contract, tconfig.Decimals, tconfig.DevWallets);
                recent.Burned = await GetSumFromWallets(service, contract, tconfig.Decimals, tconfig.BurnWallets);
                recent.CirculatingSupply = recent.TotalSupply - recent.Treasury - recent.Burned;
                recent.MarketCapLive = recent.CirculatingSupply * recent.Price;
            }

            return recent;
        }

        private static async Task<List<string>> GetTrendingSearches() {
            using var client = new HttpClient();
            var search = new SearchClient(client, jsonSettings);

            return (await search.GetSearchTrending())
                .TrendingItems
                .Select(s => s.TrendingItem.Id)
                .ToList();
        }

        private static async Task<decimal> GetSumFromWallets(BscScanTokensService service, string contract, int decimals, List<string> wallets) {
            decimal sum = 0M;
            for (int i = 0; i < wallets.Count; i++) {
                string wallet = wallets[i];

                var result = await service.GetBep20TokenAccountBalanceByContractAddressAsync(contract, wallet);
                await Task.Delay(500);

                string balance = result.Result;
                if (string.IsNullOrEmpty(balance))
                    continue;

                if (decimals > 0)
                    balance = balance[..^decimals] + "." + balance[^decimals..];

                if (decimal.TryParse(balance, out decimal value))
                    sum += value;
            }
            return sum;
        }
    }
}