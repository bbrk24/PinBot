using Discord;

namespace PinBot.Util;

public static class TaskExtensions
{
    public static async Task DeferIfNeeded(
        this Task baseTask,
        IDiscordInteraction interaction,
        DateTimeOffset? expiration = null,
        bool ephemeral = false
    )
    {
        if (interaction.HasResponded)
        {
            await baseTask;
            return;
        }

        var delay =
            (expiration ?? interaction.CreatedAt.AddSeconds(2)) - DateTimeOffset.UtcNow;

        if (delay <= TimeSpan.Zero)
        {
            await interaction.DeferAsync(ephemeral);
            await baseTask;
            return;
        }

        using var source = new CancellationTokenSource();

        var token = source.Token;
        var deferTask = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delay, token)
                    .ConfigureAwait(false);
                await interaction.DeferAsync(ephemeral);
            }
            catch (TaskCanceledException) { }
        });
        var baseAndCancelTask = Task.Run(async () =>
        {
            try
            {
                await baseTask;
            }
            finally
            {
                source.Cancel();
            }
        });

        await Task.WhenAll(baseAndCancelTask, deferTask);

        return;
    }

    public static async Task<T> DeferIfNeeded<T>(
        this Task<T> baseTask,
        IDiscordInteraction interaction,
        DateTimeOffset? expiration = null,
        bool ephemeral = false
    )
    {
        await DeferIfNeeded((Task)baseTask, interaction, expiration, ephemeral);
        return baseTask.Result;
    }
}
