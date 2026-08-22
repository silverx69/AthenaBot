namespace AthenaBot.Configuration
{
    public class ServerConfig : ModelBase
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }

        public ModelList<CommandConfig> Commands { get; set; }

        public ServerConfig() {
            Commands = [];
        }

        public ServerConfig(ulong id)
            : this() {
            Id = id;
        }
    }
}
