using System.Diagnostics;

namespace AthenaBot
{
    public class Program
    {
        static DiscordBot bot = null;

        static bool quiet = false;
        static Directories directories = null;

        public static async Task<int> Main(string[] args) {
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            if (ConsoleArguments.Parse<Arguments>(args) is Arguments a) {
                quiet = a.Quiet;
                if (a.Directory == ".")
                    a.Directory = AppDomain.CurrentDomain.BaseDirectory;
                directories = new Directories(a.Directory);
            }
            else directories = new Directories();

#if DEBUG
            Logging.LogLevel = LogLevel.Debug;
#endif

            Logging.LogDirectory = directories.Logs;
            Logging.LogMessage += ShowLog;

            await Logging.InfoAsync("Console", "Starting AthenaBot...");

            bot = new DiscordBot(directories);
            await bot.StartAsync();

            if (quiet)
                Console.WriteLine("AthenaBot is running, press any key to exit.");

            Console.ReadKey();
            return 0;
        }

        private static void ProcessExit(object sender, EventArgs e) {
            bot.Dispose();
        }

        private static void ShowLog(LogMessageEventArgs e) {
            if (quiet) return;
            var color = Console.ForegroundColor;
            Console.ForegroundColor = GetColorFromLevel(e.Level);
            Console.Write(e.Intro);
            Console.Write('\x20');
            Console.ForegroundColor = color;
            Console.WriteLine(e.Message);
            Debug.Write(e.Intro);
            Debug.Write('\x20');
            Debug.WriteLine(e.Message);
        }

        static ConsoleColor GetColorFromLevel(LogLevel level) {
            return level switch {
                LogLevel.Critical or LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                LogLevel.Debug => ConsoleColor.DarkGreen,
                LogLevel.Info => ConsoleColor.White,
                _ => Console.ForegroundColor,
            };
        }
    }
}