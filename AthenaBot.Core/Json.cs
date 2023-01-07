using AthenaBot.Converters;
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
                Converters = {
                    new JsonStringEnumConverter(),
                    new JsonByteArrayConverter()
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

        public static T Deserialize<T>(string input) {
            return JsonSerializer.Deserialize<T>(input, Options);
        }
    }
}
