namespace OpenSeaPlugin
{
    public class CollectionInfo
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public string Symbol { get; set; }

        public string Thumbnail { get; set; }

        public long Count { get; set; }

        public long Owners { get; set; }

        public double Volume30d { get; set; }

        public double FloorPrice { get; set; }

        public DateTime LastUpdate { get; set; }

        public CollectionInfo() { }

        public CollectionInfo(string slug) {
            Slug = slug;
        }
    }
}
