using AthenaBot;

namespace LibreTranslatePlugin.LibreTranslate
{
    public class TranslateResponse : ModelBase
    {
        public string TranslatedText { get; set; }

        public List<string> Alternatives { get; set; } = [];

        public DetectedLanguage DetectedLanguage { get; set; }
    }
}