using System.Text.Json;

namespace AthenaBot
{
    /// <summary>
    /// A simple class utilizing serialization to read and write typed objects to/from files as json.
    /// </summary>
    public static class Persistence
    {
        static readonly object SyncLock = new object();

        public static T LoadModel<T>(string filename) where T : new() {
            if (!File.Exists(filename))
                return new T();

            T ret = default;

            using (var sr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
                ret = Json.Deserialize<T>(sr.ReadToEnd());

            return ret;
        }

        public static async Task<T> LoadModelAsync<T>(string filename) where T : new() {
            if (!File.Exists(filename))
                return new T();
            using var stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            return await JsonSerializer.DeserializeAsync<T>(stream, Json.Options);
        }

        public static void SaveModel<T>(T model, string filename) {
            string content = Json.Serialize(model);
            using var sw = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.Write));

            sw.Write(content);
            sw.Flush();
        }

        public static async Task SaveModelAsync<T>(T model, string filename) {
            using var stream = File.Open(filename, FileMode.Create, FileAccess.Write);
            await JsonSerializer.SerializeAsync(stream, model, Json.Options);
            await stream.FlushAsync();
        }
    }
}
