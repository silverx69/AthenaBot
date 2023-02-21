using Discord.Interactions;

namespace AthenaBot.Commands
{
    public abstract class DiscordBotSilentInteractionModule : DiscordBotInteractionModule
    {
        public override async Task BeforeExecuteAsync(ICommandInfo command) {
            await DeferAsync(ephemeral: true);
            if (Context.Guild != null && !Context.ValidateCommandRoles(command))
                throw new InteractionException(command, Context, "User does not have permission to perform the command.");
        }
    }
}
