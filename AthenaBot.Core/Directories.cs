using System.Text.Json.Serialization;

namespace AthenaBot
{
    public class Directories : ModelBase
    {
        string appData;

        [JsonPropertyName("data")]
        public string AppData {
            get { return appData; }
            set {
                if (appData != value) {
                    appData = value;
                    RaisePropertyChanged(nameof(AppData));
                    RaisePropertyChanged(nameof(Logs));
                    RaisePropertyChanged(nameof(Plugins));
                }
            }
        }

        [JsonPropertyName("logs")]
        public string Logs {
            get { return Path.Combine(AppData, "Logs"); }
        }

        [JsonPropertyName("plugins")]
        public string Plugins {
            get { return Path.Combine(AppData, "Plugins"); }
        }

        public static string BaseDirectory {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public Directories()
            : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AthenaBot")) {
        }

        public Directories(string appDataDirectory) {
            AppData = appDataDirectory;
        }

        public void EnsureExists() {
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!Directory.Exists(Logs)) Directory.CreateDirectory(Logs);
            if (!Directory.Exists(Plugins)) Directory.CreateDirectory(Plugins);
        }
    }
}
