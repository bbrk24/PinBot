using Discord.WebSocket;
using PinBot.Models;
using PinBot.Services;

namespace PinBot;

public interface IReadyHandler
{
    Task ReadyAsync();
}

public class ReadyHandler : IReadyHandler
{
    private readonly IMessageService _messageService;
    private readonly ICommandsService _commandsService;
    private readonly DiscordSocketClient _client;

    public ReadyHandler(
        IMessageService messageService,
        ICommandsService commandsService,
        DiscordSocketClient client
    )
    {
        _messageService = messageService;
        _commandsService = commandsService;
        _client = client;
    }

    public async Task ReadyAsync()
    {
        await RegisterCommands();
        await SendStartupMessage();
    }

    private async Task SendStartupMessage()
    {
        await _messageService.SendMessageForEvent(
            $"Rebooted at <t:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}:T>",
            EventName.Start
        );
    }

    private async Task RegisterCommands()
    {
        await _commandsService.RegisterCommandsAsync()
            .ConfigureAwait(false);
        _client.InteractionCreated += _commandsService.ReceiveInteractionAsync;
    }
}
