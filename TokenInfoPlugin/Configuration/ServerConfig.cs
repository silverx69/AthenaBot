using AthenaBot;

namespace TokenInfoPlugin.Configuration
{
    public class ServerConfig : ModelBase
    {
        /// <summary>
        /// The Id of the server to configure settings for.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Used for organizational purposes in config file to differentiate servers easily.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// True to allow looking up any token on CoinGecko, false to only allow the configured 'default' token.
        /// </summary>
        public bool AnyToken { get; set; }

        /// <summary>
        /// True to show the configured "default" token price as the Bot's nickname.
        /// </summary>
        public bool PriceNickname { get; set; }

        /// <summary>
        /// Individual token settings. When 'AnyToken' is true, these settings will expand dynamically as users request new tokens.
        /// </summary>
        public List<TokenConfig> Tokens { get; set; }

        public ServerConfig() {
            AnyToken = true;
            Tokens = new List<TokenConfig>();
        }

        public ServerConfig(ulong guildId)
            : this() {
            Id = guildId;
        }
    }
}
