using Discord;
using PinBot.Repositories;

namespace PinBot.Services;

public interface ISettingsService
{
    Task<bool> IsPinReactionAsync(IEmote emoji, ulong guildId);
    Task<bool> IsUnpinReactionAsync(IEmote emoji, ulong guildId);
    Task<bool> RequiresForumThreadsAsync(ulong guildId);
}

public class SettingsService : ISettingsService
{
    private readonly IServerSettingsRepository _serverSettingsRepository;

    public SettingsService(IServerSettingsRepository serverSettingsRepository)
    {
        _serverSettingsRepository = serverSettingsRepository;
    }

    public async Task<bool> IsPinReactionAsync(IEmote emoji, ulong guildId)
    {
        var settings = await _serverSettingsRepository.GetSettingsAsync((long)guildId);
        return emoji.ToString() == settings.PinEmoji;
    }

    public async Task<bool> IsUnpinReactionAsync(IEmote emoji, ulong guildId)
    {
        var settings = await _serverSettingsRepository.GetSettingsAsync((long)guildId);
        return emoji.ToString() == settings.UnpinEmoji;
    }

    public async Task<bool> RequiresForumThreadsAsync(ulong guildId)
    {
        var settings = await _serverSettingsRepository.GetSettingsAsync((long)guildId);
        return settings.ForumsOnly;
    }
}
