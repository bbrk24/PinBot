using Discord;
using Discord.WebSocket;
using PinBot.Repositories;
using PinBot.Models;

namespace PinBot.Services;

public interface IMessageService
{
    Task SendMessageForEvent(string message, EventName eventName);
}

public class MessageService: IMessageService
{
    private readonly IEventChannelRepository _eventChannelRepo;
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _client;

    public MessageService(
        IEventChannelRepository eventChannelRepo,
        ILogger<MessageService> logger,
        DiscordSocketClient client
    )
    {
        _eventChannelRepo = eventChannelRepo;
        _logger = logger;
        _client = client;
    }

    public async Task SendMessageForEvent(string message, EventName eventName)
    {
        var eventChannel = await _eventChannelRepo.GetChannelForEventAsync(eventName)
            .ConfigureAwait(false);
        if (eventChannel is null)
        {
            _logger.LogInformation("Could not find channel for event {0}", eventName);
            return;
        }

        var channel = await _client.GetChannelAsync((ulong)eventChannel.Channel)
            .ConfigureAwait(false);

        if (channel is null)
        {
            _logger.LogWarning(
                "Could not find text channel with ID {0} (for event {1})",
                eventChannel.Channel,
                eventName
            );
            return;
        }

        try
        {
            if (channel is IMessageChannel messageChannel)
            {
                await messageChannel.SendMessageAsync(message)
                    .ConfigureAwait(false);
                _logger.LogDebug(
                    "Sent message for event {0} to channel {1}",
                    eventName,
                    messageChannel
                );
                return;
            }
            _logger.LogWarning(
                "Channel {0} (for event {1}) is not a text channel",
                channel,
                eventName
            );
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error sending message to channel {0} (for event {1})",
                channel,
                eventName
            );
        }
    }
}
