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

#pragma warning disable ASP0000
// How else do I get one, pray tell?
var serviceProvider = services.BuildServiceProvider();
#pragma warning restore

var config = serviceProvider.GetRequiredService<AppConfig>();
var loggingService = serviceProvider.GetRequiredService<ILoggingService>();
var readyHandler = serviceProvider.GetRequiredService<IReadyHandler>();
var eventsService = serviceProvider.GetRequiredService<IEventsService>();

client.Log += loggingService.LogAsync;
client.Ready += readyHandler.ReadyAsync;

client.ReactionAdded += eventsService.ReactionAdd;

await client.LoginAsync(TokenType.Bot, config.Token);
await client.StartAsync();

// Start Roomba'ing
var roombaService = serviceProvider.GetRequiredService<IRoombaService>();
while (true)
{
    await roombaService.RunAllRoombasAsync();
    await Task.Delay(TimeSpan.FromDays(30));
}
