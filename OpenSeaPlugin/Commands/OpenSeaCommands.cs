using Discord;
using OpenSeaClient;

namespace OpenSeaPlugin.Commands
{
    static class OpenSeaCommands
    {
        const string OpenSeaUri = "https://opensea.io/collection/{0}";

        public static string ValidateCollection(ulong guildId, string collection) {
            var server = OpenSeaPlugin.Config.Servers.Find(s => s.Id == guildId);
            if (server == null) {
                server = new Configuration.ServerConfig();
                OpenSeaPlugin.Config.Servers.Add(server);
            }
            if (!server.AnyCollection || string.IsNullOrWhiteSpace(collection)) {
                var config = server.Collections.Find(s => s.Default);
                return config?.Slug ?? string.Empty;
            }
            return collection;
        }

        public static async Task<Embed> GetStatsAsync(string collection) {
            if (string.IsNullOrWhiteSpace(collection))
                throw new ArgumentNullException(nameof(collection));

            var client = new OpenSeaHttpClient(apiKey: OpenSeaPlugin.Config.OpenSeaApiKey);

            var oscol = await client.GetCollectionAsync(collection);
            var stats = oscol.Stats;

            string symbol = oscol.AssetContracts[0].SchemaName.SchemaToSymbol();

            var eb = new EmbedBuilder {
                Title = oscol.Name,
                ThumbnailUrl = oscol.ImageUrl,
                Url = string.Format(OpenSeaUri, collection)
            };

            eb.AddField("__Items__", (long)stats.Count);
            eb.AddField("__Owners__", stats.Owners);
            eb.AddField("__Volume (30d)__", string.Format("{0:N} {1}", stats.ThirtyDayVolume, symbol));
            eb.AddField("__Floor__", string.Format("{0:N} {1}", stats.FloorPrice, symbol));
            eb.WithColor(Color.Blue);
            eb.WithCurrentTimestamp();

            return eb.Build();
        }
    }
}
