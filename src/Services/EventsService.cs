using Discord;
using Discord.WebSocket;
using PinBot.Repositories;

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
    private readonly IServerSettingsRepository _settingsRepo;

    public EventsService(ILogger<EventsService> logger, IServerSettingsRepository settingsRepo)
    {
        _logger = logger;
        _settingsRepo = settingsRepo;
    }

    public async Task ReactionAdd(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction
    )
    {
        // The channel pool will be updated a lot less frequently than the message pool,
        // so this should usually already be there.
        if (await channel.GetOrDownloadAsync() is not SocketThreadChannel threadChannel)
        {
            // Nothing to do here
            return;
        }

        // Check whether the person who reacted was the OP
        if (reaction.UserId != threadChannel.Owner.Id)
        {
            return;
        }

        var settings = await _settingsRepo.GetSettingsAsync((long)threadChannel.Guild.Id);

        // Check if it's the pin/unpin emoji
        var shouldPin = settings.PinEmoji == reaction.Emote.ToString();
        var shouldUnpin = settings.UnpinEmoji == reaction.Emote.ToString();
        if (!(shouldPin || shouldUnpin))
        {
            return;
        }

        // If the server only allows forums, check that
        if (threadChannel.ParentChannel is not IForumChannel && settings.ForumsOnly)
        {
            return;
        }

        // If the server says to ignore bots, do so
        if (settings.IgnoreBots && threadChannel.Owner.IsBot)
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
                await userMessage.RemoveAllReactionsForEmoteAsync(reaction.Emote)
                    .ConfigureAwait(false);
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
                await userMessage.RemoveAllReactionsForEmoteAsync(reaction.Emote)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while removing reaction {0}", reaction);
            }
        }
    }
}
