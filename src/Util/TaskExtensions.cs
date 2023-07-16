using Discord;

namespace PinBot.Util;

public static class TaskExtensions
{
    public static async Task<T> DeferIfNeeded<T>(
        this Task<T> baseTask,
        IDiscordInteraction interaction,
        DateTimeOffset expiration,
        bool ephemeral = false
    )
    {
        var delay = expiration - DateTimeOffset.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            await interaction.DeferAsync(ephemeral);
            return await baseTask;
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
            var retval = await baseTask;
            source.Cancel();
            return retval;
        });

        await Task.WhenAll(baseAndCancelTask, deferTask);

        return baseAndCancelTask.Result;
    }
}
