using Discord;
using Discord.Interactions;

namespace PinBot.Attributes;

internal class ModeratorAttribute : RequireUserPermissionAttribute
{
    public ModeratorAttribute(
        GuildPermission guildPermission = Discord.GuildPermission.Administrator
    ) : base(guildPermission)
    { }

    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo commandInfo,
        IServiceProvider services
    )
    {
        // Trying to be nice to the compiler here: the base class does this same check, so
        // hopefully by using the same operation rather than e.g. `context.Guild is null`
        // it can avoid double-checking.
        if (context.User is not IGuildUser)
        {
            await context.Interaction.RespondAsync(
                "That command can only be used in servers.",
                ephemeral: true
            );
            return PreconditionResult.FromError(NotAGuildErrorMessage);
        }

        var result = await base.CheckRequirementsAsync(context, commandInfo, services);
        if (!result.IsSuccess)
        {
            await context.Interaction.RespondAsync(
                "Only server mods may use that command.",
                ephemeral: true
            );
        }
        return result;
    }
}
