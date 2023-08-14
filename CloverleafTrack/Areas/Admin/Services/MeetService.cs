namespace CloverleafTrack.Areas.Admin.Services;

public interface IMeetService
{
    public Task ReloadAsync(CancellationToken token);
}

public class MeetService : IMeetService
{
    public async Task ReloadAsync(CancellationToken token)
    {
        
    }
}