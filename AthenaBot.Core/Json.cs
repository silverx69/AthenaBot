using AthenaBot.Converters;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot
{
    /// <summary>
    /// A simplified Json serializer api with some useful default options.
    /// </summary>
    public static class Json
    {
        public static readonly JsonSerializerOptions Options;

        static Json() {
            Options = new JsonSerializerOptions(JsonSerializerDefaults.Web) {
                WriteIndented = true,
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = {
                    new JsonStringEnumConverter(),
                    new JsonByteArrayConverter(),
                    new JsonColorToStringConverter(),
                    new JsonDateTimeConverter()
                }
            };
        }

        public static string Serialize(object obj) {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return JsonSerializer.Serialize(obj, obj.GetType(), Options);
        }

        public static string Serialize<T>(T obj) {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return JsonSerializer.Serialize(obj, Options);
        }

        public static Task<string> SerializeAsync(object obj) {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return SerializeAsync<object>(obj);
        }

        public static async Task<string> SerializeAsync<T>(T obj) {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using var stream = new MemoryStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            await JsonSerializer.SerializeAsync(stream, obj, Options);

            stream.Position = 0;
            return await reader.ReadToEndAsync();
        }

        public static T Deserialize<T>(string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            return JsonSerializer.Deserialize<T>(input, Options);
        }

        public static async Task<T> DeserializeAsync<T>(string input) {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8);

            await writer.WriteAsync(input);

            stream.Position = 0;
            return await JsonSerializer.DeserializeAsync<T>(stream, Options);
        }
    }
}
