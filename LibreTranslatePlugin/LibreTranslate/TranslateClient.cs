using System.Net.Http.Json;

namespace LibreTranslatePlugin.LibreTranslate
{
    public class TranslateClient : IDisposable
    {
        HttpClient client;

        public string APIKey { get; private set; }

        public string APIUrl { get; private set; }


        public TranslateClient(string apiUrl = "https://libretranslate.com") {
            APIUrl = apiUrl;
            CreateClient();
        }

        public TranslateClient(string apiUrl, string apiKey)
            : this(apiUrl) {
            APIUrl = apiUrl;
            APIKey = apiKey;
            CreateClient();
        }

        private void CreateClient() {
            client = new HttpClient() { BaseAddress = new Uri(APIUrl) };
        }

        public async Task<List<SupportedLanguage>> GetLanguagesAsync() {
            return await client.GetFromJsonAsync<List<SupportedLanguage>>("/languages");
        }

        public async Task<DetectedLanguage> DetectLanguageAsync(string text) {

            var response = await client.PostAsJsonAsync("/detect", new TranslateRequest() {
                Text = text,
                APIKey = APIKey
            });
            return await response.Content.ReadFromJsonAsync<DetectedLanguage>();
        }

        public async Task<TranslateResponse> TranslateAsync(string text, string target, string source = "auto", int alternatives = 0) {

            var response = await client.PostAsJsonAsync("/translate", new TranslateRequest() {
                Text = text,
                Target = target,
                Source = source,
                Alternatives = alternatives,
                APIKey = APIKey
            });

            return await response.Content.ReadFromJsonAsync<TranslateResponse>();

        }

        public void Dispose() {
            client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
