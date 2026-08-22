namespace AthenaBot.Configuration
{
    public class ChannelsConfig : ModelBase
    {
        public ulong Id { get; set; }

        public bool Enabled { get; set; }

        public ChannelsConfig() {
            Enabled = true;
        }

        public ChannelsConfig(ulong id, bool enabled) {
            if (id == 0)
                throw new ArgumentException("Invalid channel identifier.", nameof(id));
            Id = id;
            Enabled = enabled;
        }
    }
}
