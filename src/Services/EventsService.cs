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
    private readonly ISettingsService _settingsService;

    public EventsService(
        ILogger<EventsService> logger,
        ISettingsService settingsService
    )
    {
        _logger = logger;
        _settingsService = settingsService;
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

        // Check if it's the pin/unpin emoji
        // TODO: I think this could be rewritten to make fewer database calls. Depends on
        // what EF Core does under the hood. It may not matter.
        var shouldPin = await _settingsService.IsPinReactionAsync(
            reaction.Emote,
            threadChannel.Guild.Id
        );
        if (!(shouldPin || await _settingsService.IsUnpinReactionAsync(
            reaction.Emote,
            threadChannel.Guild.Id
        )))
        {
            return;
        }

        // If the server only allows forums, check that
        if (
            threadChannel.ParentChannel is not IForumChannel
            && await _settingsService.RequiresForumThreadsAsync(threadChannel.Guild.Id)
        )
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
