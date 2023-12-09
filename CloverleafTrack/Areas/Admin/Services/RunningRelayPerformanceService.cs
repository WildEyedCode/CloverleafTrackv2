using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IRunningRelayPerformanceService
{
    public List<RunningRelayPerformance> Performances { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(RunningRelayPerformance performance, CancellationToken token);
    public List<RunningRelayPerformance> ReadAll();
    public RunningRelayPerformance? ReadById(Guid id);
    public Task UpdateAsync(RunningRelayPerformance performance, CancellationToken token);
    public Task DeleteAsync(RunningRelayPerformance performance, CancellationToken token);
}

public class RunningRelayPerformanceService : IRunningRelayPerformanceService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public RunningRelayPerformanceService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Performances = new List<RunningRelayPerformance>();
    }
    
    public List<RunningRelayPerformance> Performances { get; private set; }

    public int Count => Performances.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        var performances = (await connection.QueryAsync<RunningRelayPerformance, RunningRelayEvent, Meet, Season, RunningRelayPerformance>(RunningRelayPerformanceQueries.AllPerformancesSql,
                (performance, @event, meet, season) =>
                {
                    performance.EventId = @event.Id;
                    performance.Event = @event;
                    performance.MeetId = meet.Id;
                    performance.Meet = meet;
                    performance.Meet.SeasonId = season.Id;
                    performance.Meet.Season = season;
                    return performance;
                },
                splitOn: "EventId,MeetId,SeasonId"))
            .OrderBy(x => x.Minutes)
            .ThenBy(x => x.Seconds)
            .ToList();
        
        foreach (var performance in performances)
        {
            performance.Athletes = (await connection.QueryAsync<Athlete>(RunningRelayPerformanceQueries.AthletesForRelayPerformanceSql, new { PerformanceId = performance.Id })).ToList();
        }

        Performances = performances;
    }

    public async Task CreateAsync(RunningRelayPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        foreach (var athleteId in performance.AthleteIds)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    RunningRelayPerformanceQueries.CreateAthleteForRelayPerformanceSql,
                    new { Id = Guid.NewGuid(), AthleteId = athleteId, PerformanceId = performance.Id, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
                    cancellationToken: token
                    ));
        }

        await ReloadAsync(token);
    }

    public List<RunningRelayPerformance> ReadAll()
    {
        return Performances;
    }

    public RunningRelayPerformance? ReadById(Guid id)
    {
        return Performances.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(RunningRelayPerformance performance, CancellationToken token)
    {
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayPerformanceQueries.UpdatePerformanceSql,
                performance,
                cancellationToken: token));

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayPerformanceQueries.RemoveAllAthletesForRelayPerformanceSql,
                new { PerformanceId = performance.Id },
                cancellationToken: token));
        
        foreach (var athleteId in performance.AthleteIds)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    RunningRelayPerformanceQueries.CreateAthleteForRelayPerformanceSql,
                    new { Id = Guid.NewGuid(), AthleteId = athleteId, PerformanceId = performance.Id, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
                    cancellationToken: token
                ));
        }

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(RunningRelayPerformance performance, CancellationToken token)
    {
        performance.Deleted = true;
        performance.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayPerformanceQueries.DeletePerformanceSql,
                performance,
                cancellationToken: token));

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayPerformanceQueries.DeleteAllAthletesForRelayPerformanceSql,
                new { PerformanceId = performance.Id },
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
}