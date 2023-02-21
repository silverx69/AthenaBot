using AthenaBot;

namespace TranslatePlugin.Configuration
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
        /// The default language of the server. Used for parameterless translations.
        /// </summary>
        public string Language { get; set; }
    }
}
