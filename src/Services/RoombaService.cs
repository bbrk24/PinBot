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

    public RoombaService(
        IServiceProvider provider
    )
    {
        _provider = provider;
    }

    public async Task RunAllRoombasAsync()
    {
        IEnumerable<IRoomba> roombas = _provider.GetServices<IRoomba>();

        await Task.WhenAll(roombas.Select(r => r.RoombaAsync()));
    }
}
