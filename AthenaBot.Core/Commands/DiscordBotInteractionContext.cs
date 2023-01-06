using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
