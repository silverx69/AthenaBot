using Discord;
using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaBot.Commands
{
    public sealed class InteractionException : Exception
    {
        public ICommandInfo Command {
            get;
            private set;
        }

        public IInteractionContext Context {
            get;
            private set;
        }

        public InteractionException(ICommandInfo command, IInteractionContext context, string message)
            : base(message) {
            Command = command;
            Context = context;
        }
    }
}
