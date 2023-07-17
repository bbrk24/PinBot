using Discord;

namespace PinBot.Services;

public interface IEmojiService
{
    Task<bool> IsPinReaction(IEmote emoji, ulong guildId);
    Task<bool> IsUnpinReaction(IEmote emoji, ulong guildId);
}

public class EmojiService : IEmojiService
{
    public Task<bool> IsPinReaction(IEmote emoji, ulong guildId) =>
        Task.FromResult(emoji.Name == "📌");

    public Task<bool> IsUnpinReaction(IEmote emoji, ulong guildId) =>
        Task.FromResult(emoji.Name == "🚫");
}
