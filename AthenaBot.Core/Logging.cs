using Discord;
using System.Collections.Concurrent;
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

    public class LogMessageEventArgs : EventArgs
    {
        public LogLevel Level { get; private set; }

        public string Intro { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }

        internal LogMessageEventArgs(LogLevel level, string intro, string message) {
            Level = level;
            Intro = intro;
            Message = message;
        }
    }

    /// <summary>
    /// A class to simplify logging messages to the various outputs, Debug, Console and hard disk.
    /// </summary>
    public static class Logging
    {
        static readonly DateTime InitTime;

        static volatile bool isWriting = false;
        static readonly ConcurrentQueue<LogMessageEventArgs> pendingWrites;

        const string LineFormat = "{0} {1}";
        const string IntroFormat = "[{0}] [{1}] [{2}]";
        const string ErrorFormat = "{0}: {1}{2}{3}";
        const string ErrorFormatNoDebug = "{0}: {1}";

        public static LogLevel LogLevel { get; set; }

        public static string LogDirectory { get; set; }

        static Logging() {
            InitTime = DateTime.Now;
            isWriting = false;
            pendingWrites = new ConcurrentQueue<LogMessageEventArgs>();
            LogLevel = LogLevel.Info;
            LogDirectory = ".";
        }

        public static void Critical(string source, Exception ex) {
            WriteLine(LogLevel.Critical, source, ErrorFormat, ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
        }

        public static Task CriticalAsync(string source, Exception ex) {
            return WriteLineAsync(LogLevel.Critical, source, ErrorFormat, ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
        }

        public static void Error(string source, Exception ex) {
            Error(LogLevel.Error, source, ex);
        }

        public static void Error(LogLevel level, string source, Exception ex) {
            if (LogLevel >= LogLevel.Debug)
                WriteLine(level, source, ErrorFormat, ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
            else
                WriteLine(level, source, ErrorFormatNoDebug, ex.GetType().Name, ex.Message);
        }

        public static Task ErrorAsync(string source, Exception ex) {
            return ErrorAsync(LogLevel.Error, source, ex);
        }

        public static Task ErrorAsync(LogLevel level, string source, Exception ex) {
            if (LogLevel >= LogLevel.Debug)
                return WriteLineAsync(level, source, ErrorFormat, ex.GetType().Name, ex.Message, Environment.NewLine, ex.StackTrace);
            else
                return WriteLineAsync(level, source, ErrorFormatNoDebug, ex.GetType().Name, ex.Message);
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
                pendingWrites.Enqueue(BuildLine(level, source, format, args));
                if (!isWriting) {
                    isWriting = true;
                    using var writer = GetWriter();
                    await ClearQueueAsync(writer);
                    await writer.FlushAsync();
                    await writer.DisposeAsync();
                    isWriting = false;
                }
            }
        }

        private static async Task ClearQueueAsync(StreamWriter writer) {
            while (pendingWrites.TryDequeue(out LogMessageEventArgs e)) {
                LogMessage?.Invoke(e);
                await writer.WriteLineAsync(string.Format(LineFormat, e.Intro, e.Message));
            }
        }

        private static LogMessageEventArgs BuildLine(LogLevel level, string source, string format, params object[] args) {
            return new LogMessageEventArgs(
                level,
                string.Format(IntroFormat, DateTime.Now.ToString(), source, level),
                string.Format(format, args));
        }

        private static StreamWriter GetWriter() {
            return new StreamWriter(File.Open(GetFilename(), FileMode.Append, FileAccess.Write), Encoding.UTF8);
        }

        private static string GetFilename() {
            return Path.Combine(LogDirectory, InitTime.ToString("yyyy-MM-dd") + ".txt");
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
        public delegate void LogMessageHandler(LogMessageEventArgs e);
    }
}