using AthenaBot;

namespace LibreTranslatePlugin.LibreTranslate
{
    public class SupportedLanguage : ModelBase
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }

    public class DetectedLanguage : ModelBase
    {
        public string Language { get; set; }

        public double Confidence { get; set; }
    }
}
