using AthenaBot;
using System.Diagnostics;

static void ShowLog(LogLevel level, string intro, string message) {
    static ConsoleColor GetColorFromLevel(LogLevel level) {
        return level switch {
            LogLevel.Critical or LogLevel.Error => ConsoleColor.Red,
            LogLevel.Warning => ConsoleColor.DarkYellow,
            LogLevel.Debug => ConsoleColor.DarkGreen,
            LogLevel.Info => ConsoleColor.White,
            _ => Console.ForegroundColor,
        };
    }
    var color = Console.ForegroundColor;
    Console.ForegroundColor = GetColorFromLevel(level);
    Console.Write(intro);
    Console.Write('\x20');
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Debug.Write(intro);
    Debug.Write('\x20');
    Debug.WriteLine(message);
}

var d = new Directories();
Logging.LogDirectory = d.Logs;
Logging.LogMessage += ShowLog;

await Logging.InfoAsync("Console", "Starting AthenaBot...");

using var bot = new DiscordBot(d);
await bot.StartAsync();

Console.ReadKey();