using Discord;

namespace PinBot.Util;

public static class CommandExtensions
{
    public static async Task RespondOrEditAsync(
        this IDiscordInteraction interaction,
        string message,
        bool ephemeral = false
    )
    {
        if (interaction.HasResponded)
        {
            await interaction.FollowupAsync(message, ephemeral: ephemeral);
        }
        else
        {
            await interaction.RespondAsync(message, ephemeral: ephemeral);
        }
    }
}
