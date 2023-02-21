using AthenaBot;
using Discord;
using System.Globalization;

namespace TranslatePlugin.Commands
{
    static class TranslateCommands
    {
        static List<CultureInfo> Languages;

        public static async Task<Embed> TranslateAsync(ulong guildId, string text, string to = null, string from = null) {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (TranslatePlugin.Self.Client == null)
                throw new InvalidOperationException("Attempted to call Translate without a valid configuration.");

            if (await LoadLanguageListAsync())
                await Task.Delay(1000);

            if (string.IsNullOrWhiteSpace(to)) {
                var server = TranslatePlugin.Config.Servers.Find(s => s.Id == guildId);
                to = ToLanguageCode(server?.Language);
            }
            else to = ToLanguageCode(to);

            if (!string.IsNullOrWhiteSpace(from))
                from = ToLanguageCode(from);

            else from = null;

            var result = await TranslatePlugin.Self.Client.TranslateTextAsync(text, to, from);

            var eb = new EmbedBuilder();

            eb.AddField(
                string.Format("Source: {0}", FromLanguageCode(result.DetectedSourceLanguage)),
                text);
            eb.AddField(
                string.Format("Result: {0}", FromLanguageCode(result.TargetLanguage)),
                result.TranslatedText);

            return eb.Build();
        }

        private static async Task<bool> LoadLanguageListAsync() {
            if (Languages != null)
                return false;

            var fileInfo = new FileInfo(Path.Combine(TranslatePlugin.Self.Directory, "langs.json"));

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
                var response = await TranslatePlugin.Self.Client.ListLanguagesAsync();
                results = response.Select(s => s.Code).ToList();
                await Persistence.SaveModelAsync(results, fileInfo.FullName);
            }

            Languages = new List<CultureInfo>();

            foreach (string language in results) {
                try {
                    Languages.Add(CultureInfo.GetCultureInfo(language));
                }
                catch (CultureNotFoundException cnf) {
                    await Logging.ErrorAsync("TranslatePlugin", cnf);
                }
            }

            return !cached;
        }

        private static string ToLanguageCode(string language) {
            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentNullException(nameof(language));
            if (Languages != null) {
                language = language.ToUpper();

                var lang = Languages.Find(s => s.Name.ToUpper() == language || s.EnglishName.ToUpper() == language);
                lang ??= Languages.Find(s => s.EnglishName.StartsWith(language + " "));

                if (lang != null) return lang.Name;
            }
            return "en";
        }

        private static string FromLanguageCode(string code) {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            if (Languages != null) {
                code = code.ToUpper();

                var ci = Languages.Find(s => s.Name.ToUpper() == code);
                if (ci != null) return ci.EnglishName;
            }
            return "English";
        }
    }
}
