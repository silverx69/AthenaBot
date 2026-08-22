using AthenaBot;
using AthenaBot.Plugins;
using LibreTranslatePlugin.Configuration;
using LibreTranslatePlugin.LibreTranslate;
using System.Globalization;

namespace LibreTranslatePlugin
{
    public class LibreTranslatePlugin : DiscordBotPlugin
    {
        static List<CultureInfo> Languages;

        public TranslateClient Client {
            get;
            private set;
        }

        internal static PluginConfig Config {
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

            Config = Persistence.LoadModel<PluginConfig>(file);
            if (!File.Exists(file)) Persistence.SaveModel(Config, file);

            try {
                Client = new TranslateClient(Config.APIUrl, Config.APIKey);
            }
            catch (Exception ex) {
                Logging.Error("LibreTranslatePlugin", ex);
            }
        }

        public override void OnPluginKilled() {
            Client?.Dispose();
            Persistence.SaveModel(Config, Path.Combine(Directory, "config.json"));
        }

        public async Task<bool> LoadLanguageListAsync() {
            if (Languages != null)
                return false;

            var fileInfo = new FileInfo(Path.Combine(Directory, "langs.json"));

            bool cached = false;
            List<string> results = null;

            if (fileInfo.Exists) {
                DateTime now = DateTime.UtcNow;
                if (now.Subtract(fileInfo.LastWriteTimeUtc).TotalDays < 7) {
                    cached = true;
                    results = await Persistence.LoadModelAsync<List<string>>(fileInfo.FullName);
                }
            }

            if (!cached) {
                var response = await Client.GetLanguagesAsync();
                results = [.. response.Select(s => s.Code)];
                await Persistence.SaveModelAsync(results, fileInfo.FullName);
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

            return !cached;
        }

        public async Task<TranslateResponse> TranslateAsync(string text, string to, string from = "auto") {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (Client == null)
                throw new InvalidOperationException("Attempted to call Translate without a valid configuration.");

            if (await LoadLanguageListAsync())
                await Task.Delay(1000);

            to = ToLanguageCode(to);

            if (from != "auto")
                from = ToLanguageCode(from);

            return await Client.TranslateAsync(text, to, from);
        }

        public static string ToLanguageCode(string language) {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));

            if (Languages != null) {
                language = language.Split("-")[0];

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
