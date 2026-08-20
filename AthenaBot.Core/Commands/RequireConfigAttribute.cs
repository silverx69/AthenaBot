using Discord.Commands;

namespace AthenaBot.Commands
{
    public sealed class RequireConfigAttribute : PreconditionAttribute
    {
        public RequireConfigAttribute() { }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services) {
            if (context is AthenaCommandContext ctx) {
                if (!ctx.ValidateCommandRoles(command))
                    return Task.FromResult(PreconditionResult.FromError("User does not have permission to perform the command."));
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            return Task.FromResult(PreconditionResult.FromError("Command was not of type \"AthenaCommandContext\""));
        }
    }
}
