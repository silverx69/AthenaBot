using Discord.Interactions;
using Discord.WebSocket;

namespace AthenaBot.Commands
{
    public sealed class DiscordBotInteractionContext : SocketInteractionContext
    {
        public DiscordBot Bot {
            get;
            private set;
        }

        public DiscordBotInteractionContext(DiscordBot bot, SocketInteraction interaction)
            : base(bot.Client, interaction) {
            Bot = bot;
        }

        public bool ValidateCommandRoles(ICommandInfo command) {
            return Bot.ValidateCommandRoles(this, command);
        }
    }
}
