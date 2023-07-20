using DataOnion.db;
using Npgsql;
using PinBot.Models;
using PinBot.Services;
using PinBot.Util;

namespace PinBot.Repositories;

public interface IServerSettingsRepository
{
    Task<ServerSettings> GetSettingsAsync(long serverId);
    Task DeleteSettingsAsync(long serverId, ServerSettings? settings = null);

    Task UpdatePinEmojiAsync(long serverId, string emoji);
    Task UpdateUnpinEmojiAsync(long serverId, string emoji);
    Task UpdateAllEmojisAsync(long serverId, string emoji);

    Task SetForumsOnlyAsync(long serverId, bool forumsOnly);
}

public class ServerSettingsRepository : IServerSettingsRepository, IRoomba
{
    private readonly IEFCoreService<ApplicationContext> _efCoreService;
    private readonly IDapperService<NpgsqlConnection> _dapperService;

    public ServerSettingsRepository(
        IEFCoreService<ApplicationContext> efCoreService,
        IDapperService<NpgsqlConnection> dapperService
    )
    {
        _efCoreService = efCoreService;
        _dapperService = dapperService;
    }

    public async Task UpdatePinEmojiAsync(long serverId, string emoji)
    {
        await _efCoreService.UpsertAsync(
            serverId,
            (ServerSettings settings) => settings.PinEmoji = emoji
        );
    }

    public async Task UpdateUnpinEmojiAsync(long serverId, string emoji)
    {
        await _efCoreService.UpsertAsync(
            serverId,
            (ServerSettings settings) => settings.UnpinEmoji = emoji
        );
    }

    public async Task DeleteSettingsAsync(long serverId, ServerSettings? settings)
    {
        settings ??= await _efCoreService.FetchAsync<ServerSettings, long>(serverId);
        if (settings is not null)
        {
            await _efCoreService.DeleteAsync(settings);
        }
    }

    public async Task UpdateAllEmojisAsync(long serverId, string emoji)
    {
        await _efCoreService.UpsertAsync(serverId, (ServerSettings settings) =>
        {
            settings.UnpinEmoji = emoji;
            settings.PinEmoji = emoji;
        });
    }

    public async Task<ServerSettings> GetSettingsAsync(long serverId)
    {
        var result = await _efCoreService.FetchAsync<ServerSettings, long>(serverId);
        return result ?? new ServerSettings { Id = serverId };
    }

    public async Task SetForumsOnlyAsync(long serverId, bool forumsOnly)
    {
        await _efCoreService.UpsertAsync(
            serverId,
            (ServerSettings settings) => settings.ForumsOnly = forumsOnly
        );
    }

    public async Task RoombaAsync()
    {
        await _dapperService.ExecuteAsync(
            @"DELETE FROM server_settings
WHERE pin_emoji = @PinEmoji AND unpin_emoji = @UnpinEmoji AND forums_only = @ForumsOnly",
            new ServerSettings()
        );
    }
}
