using Discord;
using PinBot.Repositories;

namespace PinBot.Services;

public interface IEmojiService
{
    Task<bool> IsPinReaction(IEmote emoji, ulong guildId);
    Task<bool> IsUnpinReaction(IEmote emoji, ulong guildId);
}

public class EmojiService : IEmojiService
{
    private readonly IServerSettingsRepository _serverSettingsRepository;

    public EmojiService(IServerSettingsRepository serverSettingsRepository)
    {
        _serverSettingsRepository = serverSettingsRepository;
    }

    public async Task<bool> IsPinReaction(IEmote emoji, ulong guildId)
    {
        var settings = await _serverSettingsRepository.GetSettingsAsync((long)guildId);
        return emoji.ToString() == settings.PinEmoji;
    }

    public async Task<bool> IsUnpinReaction(IEmote emoji, ulong guildId)
    {
        var settings = await _serverSettingsRepository.GetSettingsAsync((long)guildId);
        return emoji.ToString() == settings.UnpinEmoji;
    }
}
