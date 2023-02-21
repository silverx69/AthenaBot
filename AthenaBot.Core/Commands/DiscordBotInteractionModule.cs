using Discord.Interactions;

namespace AthenaBot.Commands
{
    public abstract class DiscordBotInteractionModule : InteractionModuleBase<DiscordBotInteractionContext>
    {
        public override async Task BeforeExecuteAsync(ICommandInfo command) {
            await DeferAsync();
            if (Context.Guild != null && !Context.ValidateCommandRoles(command)) {
                await DeleteOriginalResponseAsync();
                throw new InteractionException(command, Context, "User does not have permission to perform the command.");
            }
        }
    }
}
