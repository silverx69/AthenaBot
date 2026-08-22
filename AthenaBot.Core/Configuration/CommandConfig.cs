namespace AthenaBot.Configuration
{
    public class CommandConfig : ModelBase
    {
        public string Name { get; set; }

        public bool AdminOnly { get; set; }

        public bool Enabled { get; set; }

        public ModelList<string> Roles { get; set; }

        public ModelList<ChannelsConfig> Channels { get; set; }

        public CommandConfig() {
            Enabled = true;
            Roles = [];
            Channels = [];
        }

        public CommandConfig(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
            Enabled = true;
            Roles = [];
            Channels = [];
        }
    }
}
