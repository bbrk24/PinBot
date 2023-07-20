using Discord.Interactions;
using Discord.WebSocket;
using PinBot.Models;
using System.Reflection;

namespace PinBot.Services;

public interface ICommandsService
{
    Task RegisterCommandsAsync();
    Task ReceiveInteractionAsync(SocketInteraction interaction);
}

public class CommandsService : ICommandsService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _client;
    private readonly AppConfig _config;
    private readonly ILogger _logger;
    private readonly InteractionService _interactionService;

    public CommandsService(IServiceProvider provider)
    {
        _provider = provider;
        _client = provider.GetRequiredService<DiscordSocketClient>();
        _config = provider.GetRequiredService<AppConfig>();
        _logger = provider.GetRequiredService<ILogger<CommandsService>>();

        _interactionService = new InteractionService(
            _client,
            new InteractionServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                AutoServiceScopes = true,
                UseCompiledLambda = true
            }
        );
    }

    public async Task RegisterCommandsAsync()
    {
        await _interactionService.AddModulesAsync(
            Assembly.GetEntryAssembly(),
            _provider
        );

        if (_config.ServerId is null)
        {
            await _interactionService.RegisterCommandsGloballyAsync();
        }
        else
        {
            await _interactionService.RegisterCommandsToGuildAsync(
                _config.ServerId.Value
            );
        }
    }

    public async Task ReceiveInteractionAsync(SocketInteraction interaction)
    {
        LogInteraction(interaction);
        var context = new SocketInteractionContext(_client, interaction);
        await _interactionService.ExecuteCommandAsync(context, _provider);
    }

    private void LogInteraction(SocketInteraction interaction)
    {
        string description = interaction switch
        {
            SocketCommandBase cmd => '/' + cmd.CommandName,
            _ => interaction.Type.ToString()
        };
        _logger.LogDebug("Received interaction: {0}", description);
    }
}
