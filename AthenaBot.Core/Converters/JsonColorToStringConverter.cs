using Discord;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AthenaBot.Converters
{
    public class JsonColorToStringConverter : JsonConverter<Color>
    {
        static readonly Type ColorType;
        static readonly List<FieldInfo> ColorNames;

        static JsonColorToStringConverter() {
            ColorType = typeof(Color);
            ColorNames = ColorType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(s => s.FieldType == ColorType)
                .ToList();
        }

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            string value = reader.GetString().ToUpper();

            if (string.IsNullOrEmpty(value))
                return Color.Default;

            if (value.Length >= 7 && value.StartsWith('#'))
                return new Color(
                    Convert.ToInt32(value[1..3], 16),
                    Convert.ToInt32(value[3..5], 16),
                    Convert.ToInt32(value[5..7], 16));

            var field = ColorNames.Find(s => s.Name.ToUpper() == value);
            if (field != null)
                return (Color)field.GetValue(null);

            return Color.Default;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.ToString());
        }
    }
}
