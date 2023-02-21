using Discord.Commands;

namespace AthenaBot.Commands
{
    public abstract class DiscordBotCommandModule : ModuleBase<DiscordBotCommandContext>
    {
        protected override void BeforeExecute(CommandInfo command) {
            if (Context.Guild != null && !Context.ValidateCommandRoles(command)) {
                throw new InvalidOperationException("User does not have permission to perform the command.");
            }
        }
    }
}
