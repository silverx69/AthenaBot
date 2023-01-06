using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaBot.Commands
{
    public abstract class DiscordBotInteractionModule : InteractionModuleBase<DiscordBotInteractionContext>
    {
        public override async Task BeforeExecuteAsync(ICommandInfo command) {
            await DeferAsync();
            if (!Context.Bot.ValidateCommandRoles(
                Context.Guild,
                Context.Channel as SocketGuildChannel,
                Context.User as SocketGuildUser,
                command)) {
                await DeleteOriginalResponseAsync();
                throw new InteractionException(command, Context, "User does not have permission to perform the command.");
            }
        }
    }
}
