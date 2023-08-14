namespace CloverleafTrack.Areas.Admin.Services;

public interface IPerformanceService
{
    public Task ReloadAsync(CancellationToken token);
}

public class PerformanceService : IPerformanceService
{
    public async Task ReloadAsync(CancellationToken token)
    {
        
    }
}