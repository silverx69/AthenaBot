using Discord.Interactions;
using Discord.WebSocket;

namespace AthenaBot.Interactions
{
    public sealed class AthenaInteractionContext : SocketInteractionContext
    {
        public DiscordBot Bot {
            get;
            private set;
        }

        public AthenaInteractionContext(DiscordBot bot, SocketInteraction interaction)
            : base(bot.Client, interaction) {
            Bot = bot;
        }

        public bool ValidateCommandRoles(ICommandInfo command) {
            return Bot.ValidateCommandRoles(this, command);
        }
    }
}
