namespace LibreTranslatePlugin.Configuration
{
    public class ServerConfig
    {
        public ulong Id { get; set; }

        public string Comment { get; set; }
        /// <summary>
        /// The default language of the server. Used for parameterless translations.
        /// </summary>
        public string Language { get; set; }
    }
}