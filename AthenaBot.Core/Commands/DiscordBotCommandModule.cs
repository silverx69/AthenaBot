using Discord.Commands;
using Discord.WebSocket;

namespace AthenaBot.Commands
{
    public abstract class DiscordBotCommandModule : ModuleBase<DiscordBotCommandContext>
    {
        protected override void BeforeExecute(CommandInfo command) {
            if (!Context.Bot.ValidateCommandRoles(
                Context.Guild,
                Context.Channel as SocketGuildChannel,
                Context.User as SocketGuildUser,
                command)) {
                throw new InvalidOperationException("User does not have permission to perform the command.");
            }
        }
    }
}
