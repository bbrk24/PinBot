using Discord;
using Discord.WebSocket;

namespace PinBot.Services;

public interface IEventsService
{
    Task ReactionAdd(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction
    );
}

public class EventsService : IEventsService
{
    private readonly ILogger _logger;
    private readonly IEmojiService _emojiService;

    public EventsService(
        ILogger<EventsService> logger,
        IEmojiService emojiService
    )
    {
        _logger = logger;
        _emojiService = emojiService;
    }

    public async Task ReactionAdd(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction
    ) => await HandleReaction(message, channel, reaction).ConfigureAwait(false);

    private async Task HandleReaction(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction
    )
    {
        // The channel pool will be updated a lot less frequently than the message pool,
        // so this should usually already be there.
        if (await channel.GetOrDownloadAsync() is not IGuildChannel guildChannel)
        {
            // Nothing to do here
            return;
        }

        // Check if it's the pin/unpin emoji
        var shouldPin = await _emojiService.IsPinReaction(
            reaction.Emote,
            guildChannel.GuildId
        );
        if (!(
            shouldPin
            || await _emojiService.IsUnpinReaction(reaction.Emote, guildChannel.GuildId)
        ))
        {
            return;
        }

        // Finally, download the message info to check whether it's already pinned.
        var userMessage = await message.GetOrDownloadAsync();
        if (userMessage.IsPinned == shouldPin)
        {
            return;
        }

        if (shouldPin)
        {
            try
            {
                await userMessage.PinAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while pinning message");
                return;
            }

            try
            {
                await userMessage.RemoveAllReactionsForEmoteAsync(reaction.Emote);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while removing reaction {0}", reaction);
            }
        }
        else
        {
            try
            {
                await userMessage.UnpinAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while unpinning message");
                return;
            }

            try
            {
                await userMessage.RemoveAllReactionsForEmoteAsync(reaction.Emote);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while removing reaction {0}", reaction);
            }
        }
    }
}
