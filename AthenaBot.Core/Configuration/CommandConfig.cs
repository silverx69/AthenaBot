namespace AthenaBot.Configuration
{
    public class CommandConfig : ModelBase
    {
        public string Name { get; set; }

        public ModelList<CommandServerConfig> Servers { get; set; } = [];

        public CommandConfig() { }

        public CommandConfig(string name) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }

    public class CommandServerConfig : ModelBase
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }

        public bool Enabled { get; set; } = true;

        public bool AdminOnly { get; set; }

        public ModelList<string> Roles { get; set; } = [];

        public ModelList<CommandChannelConfig> Channels { get; set; } = [];

        public CommandServerConfig() { }

        public CommandServerConfig(ulong id, bool enabled) {
            if (id == 0)
                throw new ArgumentException("Invalid server identifier.", nameof(id));
            Id = id;
            Enabled = enabled;
        }
    }

    public class CommandChannelConfig : ModelBase
    {
        public ulong Id { get; set; }

        public bool Enabled { get; set; } = true;

        public CommandChannelConfig() { }

        public CommandChannelConfig(ulong id, bool enabled) {
            if (id == 0)
                throw new ArgumentException("Invalid channel identifier.", nameof(id));
            Id = id;
            Enabled = enabled;
        }
    }
}
