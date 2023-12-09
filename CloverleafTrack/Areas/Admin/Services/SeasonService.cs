using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface ISeasonService
{
    public List<Season> Seasons { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(Season season, CancellationToken token);
    public List<Season> ReadAll();
    public Season? ReadById(Guid id);
    public Task UpdateAsync(Season season, CancellationToken token);
    public Task DeleteAsync(Season season, CancellationToken token);
}

public class SeasonService : ISeasonService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public SeasonService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;

        Seasons = new List<Season>();
    }
    
    public List<Season> Seasons { get; private set; }
    public int Count => Seasons.Count;
    
    public async Task ReloadAsync(CancellationToken token)
    {
        Seasons = (await connection.QueryAsync<Season>(SeasonQueries.AllSeasonsSql)).ToList();
    }

    public async Task CreateAsync(Season season, CancellationToken token)
    {
        season.Id = Guid.NewGuid();
        season.DateCreated = DateTime.UtcNow;
        season.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                SeasonQueries.CreateSeasonSql,
                season,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<Season> ReadAll()
    {
        return Seasons;
    }

    public Season? ReadById(Guid id)
    {
        return Seasons.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(Season season, CancellationToken token)
    {
        season.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                SeasonQueries.UpdateSeasonSql,
                season,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(Season season, CancellationToken token)
    {
        season.Deleted = true;
        season.DateDeleted = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                SeasonQueries.DeleteSeasonSql,
                season,
                cancellationToken: token));

        await ReloadAsync(token);
    }
}