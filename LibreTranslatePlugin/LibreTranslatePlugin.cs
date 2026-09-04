using AthenaBot;
using AthenaBot.Plugins;
using LibreTranslatePlugin.Configuration;
using LibreTranslatePlugin.LibreTranslate;
using System.Globalization;

namespace LibreTranslatePlugin
{
    public class LibreTranslatePlugin : AthenaBotPlugin
    {
        static List<CultureInfo> Languages;

        public TranslateClient Client {
            get;
            private set;
        }

        internal PluginConfig Config {
            get;
            private set;
        }

        internal static LibreTranslatePlugin Self {
            get;
            private set;
        }

        public LibreTranslatePlugin() {
            Self = this;
        }

        public override void OnPluginLoaded() {
            string file = Path.Combine(Directory, "config.json");

            Config = Persistence.Load<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.Save(Config, file);

            try {
                Client = new TranslateClient(Config.APIUrl, Config.APIKey);
            }
            catch (Exception ex) {
                Logging.Error("LibreTranslatePlugin", ex);
            }
        }

        public override void OnPluginKilled() {
            Client?.Dispose();
            Persistence.Save(Config, Path.Combine(Directory, "config.json"));
        }

        public async Task LoadLanguageListAsync() {
            if (Languages != null) return;

            var fileInfo = new FileInfo(Path.Combine(Directory, "langs.json"));

            bool cached = false;
            List<string> results = null;

            if (fileInfo.Exists) {
                DateTime now = DateTime.UtcNow;
                if (now.Subtract(fileInfo.LastWriteTimeUtc).TotalDays < 7) {
                    cached = true;
                    results = await Persistence.LoadAsync<List<string>>(fileInfo.FullName);
                }
            }

            if (!cached) {
                var response = await Client.GetLanguagesAsync();
                results = [.. response.Select(s => s.Code)];
                await Persistence.SaveAsync(results, fileInfo.FullName);
            }

            Languages = [];

            foreach (string language in results) {
                try {
                    if (!Languages.Contains(CultureInfo.GetCultureInfo(language)))
                        Languages.Add(CultureInfo.GetCultureInfo(language));
                }
                catch (CultureNotFoundException cnf) {
                    await Logging.ErrorAsync("LibreTranslatePlugin", cnf);
                }
            }
        }

        public async Task<TranslateResult> TranslateAsync(string text, string to, string from = "auto") {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (Client == null)
                throw new InvalidOperationException("Attempted to call Translate without a valid configuration.");

            await LoadLanguageListAsync();

            if (from != "auto")
                from = ToLanguageCode(from);

            return await Client.TranslateAsync(text, ToLanguageCode(to), from);
        }

        public static string ToLanguageCode(string language) {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));

            if (Languages != null) {
                // in case we've been passed a locale instead of a language code
                language = language.Split('-')[0];

                var lang = Languages.Find(s =>
                    s.Name.Equals(language, StringComparison.OrdinalIgnoreCase) ||
                    s.EnglishName.Equals(language, StringComparison.OrdinalIgnoreCase) ||
                    s.NativeName.Equals(language, StringComparison.OrdinalIgnoreCase) ||
                    s.ThreeLetterISOLanguageName.Equals(language, StringComparison.OrdinalIgnoreCase));

                if (lang != null) return lang.Name;
            }
            return "en";
        }

        public static string FromLanguageCode(string code) {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            if (Languages != null) {
                var ci = Languages.Find(s => s.Name.Equals(code, StringComparison.OrdinalIgnoreCase));
                if (ci != null) return ci.EnglishName;
            }
            return "English";
        }
    }
}
