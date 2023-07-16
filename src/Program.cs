using Discord;
using Discord.WebSocket;
using PinBot;
using PinBot.Models;
using PinBot.Services;

var client = new DiscordSocketClient(new DiscordSocketConfig
{
    GatewayIntents =
        GatewayIntents.Guilds
        | GatewayIntents.GuildMessageReactions
});

var services = new ServiceCollection();

services.AddConfig();
services.AddLogger();
services.AddDatabase();
services.AddRepositories();
services.AddGeneralServices();

services.AddSingleton(client);

var serviceProvider = services.BuildServiceProvider();

var config = serviceProvider.GetRequiredService<AppConfig>();
var loggingService = serviceProvider.GetRequiredService<ILoggingService>();
var readyHandler = serviceProvider.GetRequiredService<IReadyHandler>();
var eventsService = serviceProvider.GetRequiredService<IEventsService>();

client.Log += loggingService.LogAsync;
client.Ready += readyHandler.ReadyAsync;

client.ReactionAdded += eventsService.ReactionAdd;

await client.LoginAsync(TokenType.Bot, config.Token);
await client.StartAsync();
await Task.Delay(-1);
