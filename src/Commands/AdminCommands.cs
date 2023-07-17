using Discord;
using Discord.Interactions;
using PinBot.Attributes;
using PinBot.Models;
using PinBot.Repositories;
using PinBot.Util;

namespace PinBot.Commands;

[Admin]
[Group("admin", "Commands for bot admins only")]
public class AdminCommands : InteractionModuleBase
{
    private readonly ILogger _logger;
    private readonly IEventChannelRepository _repo;

    public AdminCommands(
        ILogger<AdminCommands> logger,
        IEventChannelRepository repo
    )
    {
        _logger = logger;
        _repo = repo;
    }

    [SlashCommand("setchannel", "Set the channel for an event.")]
    public async Task SetChannelAsync(
        [Summary("event", "Which event to send")] EventName eventName,
        [ChannelTypes(ChannelType.Text, ChannelType.News)]
        [Summary("channel", "Where to send it")] ITextChannel channel
    )
    {
        try
        {
            var expiration = Context.Interaction.CreatedAt.AddSeconds(2);

            await _repo.SetChannelForEventAsync(eventName, (long)channel.Id)
                .DeferIfNeeded(Context.Interaction, expiration);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error occurred while setting channel for event {0} to {1}",
                eventName,
                channel.Id
            );
            await Context.Interaction.RespondOrEditAsync(
                "An error occurred while saving.",
                ephemeral: true
            );
            return;
        }

        await Context.Interaction.RespondOrEditAsync(
            $"Will now send messages for '{eventName}' to this channel."
        );
    }
}
