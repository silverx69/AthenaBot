using AthenaBot;

namespace OpenSeaPlugin.Configuration
{
    public class ServerConfig : ModelBase
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }

        public bool AnyCollection { get; set; }

        public List<CollectionConfig> Collections { get; set; }

        public ServerConfig() {
            AnyCollection = true;
            Collections = new List<CollectionConfig>();
        }

        public ServerConfig(ulong id)
            : this() {
            Id = id;
        }
    }
}
