using Discord.Commands;

namespace AthenaBot.Commands
{
    /// <summary>
    /// Acts as a hook for validating command permissions, specified in the configuration, at runtime. 
    /// AthenaBot's built-in modules have this attribute applied to their class definition by default.
    /// </summary>
    public sealed class AthenaPreconditionAttribute : PreconditionAttribute
    {
        public AthenaPreconditionAttribute() { }

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
