namespace PinBot.Services;

public interface IRoomba
{
    Task RoombaAsync();
}

public interface IRoombaService
{
    Task RunAllRoombasAsync();
}

public class RoombaService : IRoombaService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger? _logger;

    public RoombaService(
        IServiceProvider provider
    )
    {
        _provider = provider;
        _logger = _provider.GetService<ILogger<RoombaService>>();
    }

    public async Task RunAllRoombasAsync()
    {
        _logger?.LogDebug("Beginning roombas...");

        IEnumerable<IRoomba> roombas = _provider.GetServices<IRoomba>();
        await Task.WhenAll(roombas.Select(async r =>
        {
            try
            {
                await r.RoombaAsync();
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Exception from roomba method");
            }
        }));

        _logger?.LogDebug("Roombas finished!");
    }
}
