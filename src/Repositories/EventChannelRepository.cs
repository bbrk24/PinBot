using DataOnion.db;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
    Task RemoveChannelsAsync(long[] eventChannels);
    IQueryable<long> GetAllChannelIds();
}

public class EventChannelRepository : IEventChannelRepository
{
    private readonly IEFCoreService<ApplicationContext> _efCoreService;
    private readonly IDapperService<NpgsqlConnection> _dapperService;

    public EventChannelRepository(
        IEFCoreService<ApplicationContext> efCoreService,
        IDapperService<NpgsqlConnection> dapperService
    )
    {
        _efCoreService = efCoreService;
        _dapperService = dapperService;
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

    public async Task RemoveChannelsAsync(long[] eventChannels)
    {
        await _dapperService.ExecuteAsync(
            "DELETE FROM event_channels WHERE channel = ANY(@Ids)",
            new { Ids = eventChannels }
        );
    }

    public IQueryable<long> GetAllChannelIds() =>
        _efCoreService.QueryableWhere<EventChannel>(_ => true)
            .AsNoTracking()
            .Select(e => e.Channel)
            .Distinct();
}
