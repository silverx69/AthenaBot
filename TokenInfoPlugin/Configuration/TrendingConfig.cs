using AthenaBot;

namespace TokenInfoPlugin.Configuration
{
    public class TrendingConfig : ModelBase
    {
        public bool Enabled { get; set; }

        public List<ulong> Channels { get; set; }

        public TrendingConfig() {
            Channels = new List<ulong>();
        }
    }
}
