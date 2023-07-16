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

    public EventsService(
        ILogger<EventsService> logger
    )
    {
        _logger = logger;
    }

    public async Task ReactionAdd(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction
    )
    {

    }
}
