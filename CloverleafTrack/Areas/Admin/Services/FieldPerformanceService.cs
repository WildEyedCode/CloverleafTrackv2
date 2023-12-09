using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IFieldPerformanceService
{
    public List<FieldPerformance> Performances { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(FieldPerformance performance, CancellationToken token);
    public List<FieldPerformance> ReadAll();
    public FieldPerformance? ReadById(Guid id);
    public Task UpdateAsync(FieldPerformance performance, CancellationToken token);
    public Task DeleteAsync(FieldPerformance performance, CancellationToken token);
}

public class FieldPerformanceService : IFieldPerformanceService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public FieldPerformanceService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Performances = new List<FieldPerformance>();
    }
    
    public List<FieldPerformance> Performances { get; private set; }

    public int Count => Performances.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        Performances = (await connection.QueryAsync<FieldPerformance, FieldEvent, Meet, Season, Athlete, FieldPerformance>(FieldPerformanceQueries.AllPerformancesSql,
                (performance, @event, meet, season, athlete) =>
                {
                    performance.EventId = @event.Id;
                    performance.Event = @event;
                    performance.MeetId = meet.Id;
                    performance.Meet = meet;
                    performance.Meet.SeasonId = season.Id;
                    performance.Meet.Season = season;
                    performance.AthleteId = athlete.Id;
                    performance.Athlete = athlete;
                    return performance;
                },
                splitOn: "EventId,MeetId,SeasonId,AthleteId"))
            .OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();
    }

    public async Task CreateAsync(FieldPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<FieldPerformance> ReadAll()
    {
        return Performances;
    }

    public FieldPerformance? ReadById(Guid id)
    {
        return Performances.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(FieldPerformance performance, CancellationToken token)
    {
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldPerformanceQueries.UpdatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(FieldPerformance performance, CancellationToken token)
    {
        performance.Deleted = true;
        performance.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldPerformanceQueries.DeletePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }
}