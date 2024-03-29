using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Areas.Admin.ViewModels;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IAthleteService
{
    public List<Athlete> Athletes { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(Athlete athlete, CancellationToken token);
    public List<Athlete> ReadAll();
    public Athlete? ReadById(Guid id);
    public Task UpdateAsync(Athlete athlete, CancellationToken token);
    public Task DeleteAsync(Athlete athlete, CancellationToken token);
}

public class AthleteService : IAthleteService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public AthleteService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Athletes = new List<Athlete>();
    }

    public List<Athlete> Athletes { get; private set; }
    public int Count => Athletes.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        Athletes = (await connection.QueryAsync<Athlete>(AthleteQueries.AllAthletesSql)).ToList();
    }

    public async Task CreateAsync(Athlete athlete, CancellationToken token = default)
    {
        athlete.Id = Guid.NewGuid();
        athlete.DateCreated = DateTime.UtcNow;
        athlete.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
        AthleteQueries.CreateAthleteSql,
                    athlete,
                    cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<Athlete> ReadAll()
    {
        return Athletes;
    }

    public Athlete? ReadById(Guid id)
    {
        return Athletes.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(Athlete athlete, CancellationToken token)
    {
        athlete.DateUpdated = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                AthleteQueries.UpdateAthleteSql,
                athlete,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }

    public async Task DeleteAsync(Athlete athlete, CancellationToken token)
    {
        athlete.Deleted = true;
        athlete.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                AthleteQueries.DeleteAthleteSql,
                athlete,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
}