using AthenaBot;

namespace TokenInfoPlugin.Configuration
{
    public class ServerConfig
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }
        /// <summary>
        /// True to allow looking up any token on CoinGecko, false to only allow the configured 'default' token.
        /// </summary>
        public bool AnyToken { get; set; } = true;

        /// <summary>
        /// True to show the configured "default" token price as the Bot's nickname.
        /// </summary>
        public bool PriceNickname { get; set; }

        /// <summary>
        /// Individual token settings. When 'AnyToken' is true, these settings will expand dynamically as users request new tokens.
        /// </summary>
        public ModelList<TokenConfig> Tokens { get; set; } = [];
    }
}
