using Discord;
using Discord.Interactions;

namespace AthenaBot.Interactions
{
    public sealed class RequireConfigAttribute : PreconditionAttribute
    {
        public RequireConfigAttribute() { }

        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services) {
            if (context is AthenaInteractionContext ctx) {
                if (!ctx.ValidateCommandRoles(command))
                    return Task.FromResult(PreconditionResult.FromError("User does not have permission to perform the command."));
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Command was not of type \"AthenaInteractionContext\""));
        }
    }
}
