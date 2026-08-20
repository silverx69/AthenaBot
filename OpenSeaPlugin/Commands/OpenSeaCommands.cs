using AthenaBot;
using Discord;
using OpenSeaClient;
using OpenSeaPlugin.Configuration;

namespace OpenSeaPlugin.Commands
{
    static class OpenSeaCommands
    {
        static Dictionary<string, CollectionInfo> recentInfos;
        const string OpenSeaUri = "https://opensea.io/collection/{0}";

        static OpenSeaCommands() {
            recentInfos = new Dictionary<string, CollectionInfo>();
        }

        public static CollectionConfig[] ValidateCollection(ulong guildId, string collection) {
            var server = OpenSeaPlugin.Config.Servers.Find(s => s.Id == guildId);
            if (server == null) {
                server = new ServerConfig();
                OpenSeaPlugin.Config.Servers.Add(server);
            }
            if (!server.AnyCollection || string.IsNullOrWhiteSpace(collection))
                return server.Collections.Where(s => s.Default).ToArray();

            return new[] { new CollectionConfig() { Slug = collection } };
        }

        public static async Task<Embed> GetStatsAsync(string slug) {
            if (string.IsNullOrWhiteSpace(slug))
                throw new ArgumentNullException(nameof(slug));

            DateTime now = DateTime.UtcNow;

            if (!recentInfos.TryGetValue(slug, out CollectionInfo collection))
                collection = new CollectionInfo(slug);

            if (now.Subtract(collection.LastUpdate).TotalMinutes < 5d)
                return GetEmbed(collection);

            recentInfos[slug] = collection;
            collection.LastUpdate = now;

            var client = new OpenSeaHttpClient(apiKey: OpenSeaPlugin.Config.OpenSeaApiKey);

            var oscol = await client.GetCollectionAsync(slug);
            var stats = oscol.Stats;

            string symbol = oscol.AssetContracts[0].SchemaName.SchemaToSymbol();

            collection.Name = oscol.Name;
            collection.Thumbnail = oscol.ImageUrl;
            collection.Count = (long)stats.Count;
            collection.Owners = stats.Owners;
            collection.Volume30d = stats.ThirtyDayVolume;
            collection.FloorPrice = stats.FloorPrice;
            collection.Symbol = symbol;

            return GetEmbed(collection);
        }

        private static Embed GetEmbed(CollectionInfo collection) {
            return new EmbedBuilder()
                .WithTitle(collection.Name)
                .WithThumbnailUrl(collection.Thumbnail)
                .WithUrl(string.Format(OpenSeaUri, collection.Slug))
                .AddField("__Items__", collection.Count)
                .AddField("__Owners__", collection.Owners)
                .AddField("__Volume (30d)__", string.Format("{0:N} {1}", collection.Volume30d, collection.Symbol))
                .AddField("__Floor__", string.Format("{0:N} {1}", collection.FloorPrice, collection.Symbol))
                .WithColor(Color.Blue)
                .WithLastUpdated(collection.LastUpdate)
                .Build();
        }
    }
}
