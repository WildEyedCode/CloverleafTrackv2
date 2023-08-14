namespace CloverleafTrack.Areas.Admin.Services;

public interface ISeasonService
{
    public Task ReloadAsync(CancellationToken token);
}

public class SeasonService : ISeasonService
{
    public async Task ReloadAsync(CancellationToken token)
    {
        
    }
}