using Discord.Commands;
using Discord.WebSocket;

namespace AthenaBot.Commands
{
    public sealed class AthenaCommandContext : SocketCommandContext
    {
        public DiscordBot Bot {
            get;
            private set;
        }

        internal AthenaCommandContext(DiscordBot bot, SocketUserMessage msg)
            : base(bot.Client, msg) {
            Bot = bot;
        }

        internal bool ValidateCommandRoles(CommandInfo command) {
            return Bot.ValidateCommandRoles(this, command);
        }
    }
}
