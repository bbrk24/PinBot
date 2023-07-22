using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PinBot.Repositories;

namespace PinBot.Services;

public class EventChannelRoomba : IRoomba
{
    private readonly IEventChannelRepository _repository;
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;

    public EventChannelRoomba(
        IEventChannelRepository repository,
        ILogger<EventChannelRoomba> logger,
        DiscordSocketClient client
    )
    {
        _repository = repository;
        _logger = logger;
        _client = client;
    }

    public async Task RoombaAsync()
    {
        var channels = await _repository.GetAllChannelIds()
            .AsAsyncEnumerable()
            .WhereAwait(async c =>
                await _client.GetChannelAsync((ulong)c) is not IMessageChannel
            )
            .ToArrayAsync();
        _logger.LogInformation("Removing channels for {0} events", channels.Length);
        await _repository.RemoveChannelsAsync(channels);
    }
}
