using AthenaBot;

namespace OpenSeaPlugin.Configuration
{
    public class AlertConfig : ModelBase
    {
        public ulong ChannelId { get; set; }

        //public AssetConfig Asset { get; set; }

        public CollectionConfig Collection { get; set; } = new CollectionConfig();
    }
}
