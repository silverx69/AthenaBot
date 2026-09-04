using AthenaBot;

namespace LibreTranslatePlugin.LibreTranslate
{
    public class TranslateResult : ModelBase
    {
        public string SourceText { get; set; }

        public string SourceLanguage { get; set; }

        public string TranslatedText { get; set; }

        public string TranslatedLanguage { get; set; }

        public List<string> Alternatives { get; set; } = [];
    }
}