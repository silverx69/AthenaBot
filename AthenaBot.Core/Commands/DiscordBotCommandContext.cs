using Discord.Commands;
using Discord.WebSocket;

namespace AthenaBot.Commands
{
    public sealed class DiscordBotCommandContext : SocketCommandContext
    {
        public DiscordBot Bot {
            get;
            private set;
        }

        internal DiscordBotCommandContext(DiscordBot bot, SocketUserMessage msg) 
            : base(bot.Client, msg) {
            Bot = bot;
        }
    }
}
