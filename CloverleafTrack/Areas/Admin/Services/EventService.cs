namespace CloverleafTrack.Areas.Admin.Services;

public interface IEventService
{
    public Task ReloadAsync(CancellationToken token);
}

public class EventService : IEventService
{
    public async Task ReloadAsync(CancellationToken token)
    {
        
    }
}