namespace AthenaBot
{
    public class Directories : ModelBase
    {
        string appData;

        public string AppData {
            get { return appData; }
            set {
                if (appData != value) {
                    appData = value;
                    AppDataChanged();
                }
            }
        }

        public string Logs {
            get { return Path.Combine(AppData, "Logs"); }
        }

        public string Plugins {
            get { return Path.Combine(AppData, "Plugins"); }
        }

        public static string BaseDirectory {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public Directories(string appDataDirectory = null) {
            AppData = !string.IsNullOrEmpty(appDataDirectory) ?
                appDataDirectory :
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AthenaBot");
        }

        private void AppDataChanged() {
            EnsureExists();
            RaisePropertyChanged(nameof(AppData));
            RaisePropertyChanged(nameof(Logs));
            RaisePropertyChanged(nameof(Plugins));
        }

        public void EnsureExists() {
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!Directory.Exists(Logs)) Directory.CreateDirectory(Logs);
            if (!Directory.Exists(Plugins)) Directory.CreateDirectory(Plugins);
        }
    }
}
