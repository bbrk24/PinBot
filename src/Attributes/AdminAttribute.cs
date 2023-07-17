using Discord;
using Discord.Interactions;

namespace PinBot.Attributes;

public class AdminAttribute : RequireOwnerAttribute
{
    public override async Task<PreconditionResult> CheckRequirementsAsync(
        IInteractionContext context,
        ICommandInfo command,
        IServiceProvider services
    )
    {
        var result = await base.CheckRequirementsAsync(context, command, services);
        if (!result.IsSuccess)
        {
            await context.Interaction.RespondAsync(
                "Only the bot owner can use that command.",
                ephemeral: true
            );
        }
        return result;
    }
}
