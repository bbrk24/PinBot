using DataOnion.db;
using PinBot.Models;
using PinBot.Util;

namespace PinBot.Repositories;

public interface IEventChannelRepository
{
    Task<EventChannel?> GetChannelForEventAsync(EventName eventName);
    Task<EventChannel> SetChannelForEventAsync(
        EventName eventName,
        long channelId
    );
}

public class EventChannelRepository : IEventChannelRepository
{
    private readonly IEFCoreService<ApplicationContext> _efCoreService;

    public EventChannelRepository(
        IEFCoreService<ApplicationContext> efCoreService
    )
    {
        _efCoreService = efCoreService;
    }

    public async Task<EventChannel?> GetChannelForEventAsync(EventName eventName)
    {
        return await _efCoreService.FetchAsync<EventChannel, EventName>(eventName);
    }

    public async Task<EventChannel> SetChannelForEventAsync(
        EventName eventName,
        long channelId
    )
    {
        return await _efCoreService.UpsertAsync(
            eventName,
            (EventChannel entity) => entity.Channel = channelId
        );
    }
}
