using AthenaBot;
using System.Text.Json.Serialization;

namespace LibreTranslatePlugin.LibreTranslate
{
    public class TranslateRequest : ModelBase
    {
        [JsonPropertyName("q")]
        public string Text { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public string Format { get; set; } = "text";

        public int Alternatives { get; set; }

        [JsonPropertyName("api_key")]
        public string APIKey { get; set; }
    }
}
