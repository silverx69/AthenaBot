using Discord;
using OpenSeaClient;

namespace OpenSeaPlugin.Commands
{
    static class OpenSeaCommands
    {
        const string OpenSeaUri = "https://opensea.io/collection/{0}";

        public static string ValidateCollection(ulong guildId, string collection) {
            if (string.IsNullOrWhiteSpace(collection)) {
                var config = OpenSeaPlugin.Config.Collections.Find(s => s.Server == guildId && s.Default);
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
            eb.AddField("Items", (long)stats.Count, true);
            eb.AddField("Owners", stats.Owners, true);
            eb.AddField("Volume (30d)", string.Format("{0} {1}", (long)stats.ThirtyDayVolume, symbol), true);
            eb.AddField("Floor", string.Format("{0} {1}", stats.FloorPrice, symbol), true);
            eb.WithCurrentTimestamp();

            return eb.Build();
        }
    }
}
