using Discord;
using System.Reflection.Emit;
using System.Text;

namespace AthenaBot
{
    [Flags]
    public enum LogLevel : int
    {
        Critical = 0,
        Error = 1,
        Warning = 2,
        Info = 4,
        Debug = 8
    }

    /// <summary>
    /// A class to simplify logging messages to the various outputs, Debug, Console and hard disk.
    /// </summary>
    public static class Logging
    {
        static readonly DateTime InitTime = DateTime.Now;

        public static LogLevel LogLevel { get; set; } = LogLevel.Info;
        public static string LogDirectory { get; set; } = ".";

        public static void Critical(string source, Exception ex) {
            WriteLine(LogLevel.Critical, source, "{0}: {1}{2}{3}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
        }

        public static Task CriticalAsync(string source, Exception ex) {
            return WriteLineAsync(LogLevel.Critical, source, "{0}: {1}{2}{3}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
        }

        public static void Error(string source, Exception ex) {
            Error(LogLevel.Error, source, ex);
        }

        public static void Error(LogLevel level, string source, Exception ex) {
            if (LogLevel >= LogLevel.Debug)
                WriteLine(level, source, "{0}: {1}{2}{3}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
            else
                WriteLine(level, source, "{0}: {1}", ex.GetType().Name, ex.Message);
        }

        public static Task ErrorAsync(string source, Exception ex) {
            return ErrorAsync(LogLevel.Error, source, ex);
        }

        public static Task ErrorAsync(LogLevel level, string source, Exception ex) {
            if (LogLevel >= LogLevel.Debug)
                return WriteLineAsync(level, source, "{0}: {1}{2}{3}", ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
            else
                return WriteLineAsync(level, source, "{0}: {1}", ex.GetType().Name, ex.Message);
        }

        public static void Error(string source, string message, params object[] args) {
            WriteLine(LogLevel.Error, source, message, args);
        }

        public static Task ErrorAsync(string source, string message, params object[] args) {
            return WriteLineAsync(LogLevel.Error, source, message, args);
        }

        public static void Warning(string source, string message, params object[] args) {
            WriteLine(LogLevel.Warning, source, message, args);
        }

        public static Task WarningAsync(string source, string message, params object[] args) {
            return WriteLineAsync(LogLevel.Warning, source, message, args);
        }

        public static void Debug(string source, string message, params object[] args) {
            WriteLine(LogLevel.Debug, source, message, args);
        }

        public static Task DebugAsync(string source, string message, params object[] args) {
            return WriteLineAsync(LogLevel.Debug, source, message, args);
        }

        public static void Info(string source, string message, params object[] args) {
            WriteLine(LogLevel.Info, source, message, args);
        }

        public static Task InfoAsync(string source, string message, params object[] args) {
            return WriteLineAsync(LogLevel.Info, source, message, args);
        }

        public static void WriteLine(LogLevel level, string source, string format, params object[] args) {
            if (level <= LogLevel) {
                using var writer = GetWriter();
                writer.WriteLine(BuildLine(level, source, format, args));
                writer.Flush();
            }
        }

        public static async Task WriteLineAsync(LogLevel level, string source, string format, params object[] args) {
            if (level <= LogLevel) {
                using var writer = GetWriter();
                await writer.WriteLineAsync(BuildLine(level, source, format, args));
                await writer.FlushAsync();
            }
        }

        private static string BuildLine(LogLevel level, string source, string format, params object[] args) {
            string intro = string.Format("[{0}] [{1}] [{2}]", DateTime.Now.ToString(), source, level);
            string message = string.Format(format, args);
            LogMessage?.Invoke(level, intro, message);
            return intro + " " + message;
        }

        private static StreamWriter GetWriter() {
            return new StreamWriter(File.Open(GetFilename(), FileMode.Append, FileAccess.Write), Encoding.UTF8);
        }

        private static string GetFilename() {
            return Path.Combine(LogDirectory, InitTime.ToString("dd-MM-yyyy") + ".txt");
        }

        /// <summary>
        /// An extension for LogSeverity that converts the Discord LogSeverity to an AthenaBot LogLevel
        /// </summary>
        /// <param name="severity">The severity being converted.</param>
        /// <returns>The equivalent LogLevel value.</returns>
        public static LogLevel ToLogLevel(this LogSeverity severity) {
            return severity switch {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Info,
                _ => LogLevel.Debug,
            };
        }

        public static event LogMessageHandler LogMessage;
        public delegate void LogMessageHandler(LogLevel level, string intro, string message);
    }
}