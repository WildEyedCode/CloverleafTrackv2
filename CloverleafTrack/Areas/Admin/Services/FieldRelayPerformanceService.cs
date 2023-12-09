using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IFieldRelayPerformanceService
{
    public List<FieldRelayPerformance> Performances { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(FieldRelayPerformance performance, CancellationToken token);
    public List<FieldRelayPerformance> ReadAll();
    public FieldRelayPerformance? ReadById(Guid id);
    public Task UpdateAsync(FieldRelayPerformance performance, CancellationToken token);
    public Task DeleteAsync(FieldRelayPerformance performance, CancellationToken token);
}

public class FieldRelayPerformanceService : IFieldRelayPerformanceService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public FieldRelayPerformanceService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Performances = new List<FieldRelayPerformance>();
    }
    
    public List<FieldRelayPerformance> Performances { get; private set; }

    public int Count => Performances.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        var performances = (await connection.QueryAsync<FieldRelayPerformance, FieldRelayEvent, Meet, Season, FieldRelayPerformance>(FieldRelayPerformanceQueries.AllPerformancesSql,
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
            .OrderByDescending(x => x.Feet)
            .ThenByDescending(x => x.Inches)
            .ToList();
        
        foreach (var performance in performances)
        {
            performance.Athletes = (await connection.QueryAsync<Athlete>(FieldRelayPerformanceQueries.AthletesForRelayPerformanceSql, new { PerformanceId = performance.Id })).ToList();
        }

        Performances = performances;
    }

    public async Task CreateAsync(FieldRelayPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        foreach (var athleteId in performance.AthleteIds)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    FieldRelayPerformanceQueries.CreateAthleteForRelayPerformanceSql,
                    new { Id = Guid.NewGuid(), AthleteId = athleteId, PerformanceId = performance.Id, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
                    cancellationToken: token
                    ));
        }

        await ReloadAsync(token);
    }

    public List<FieldRelayPerformance> ReadAll()
    {
        return Performances;
    }

    public FieldRelayPerformance? ReadById(Guid id)
    {
        return Performances.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(FieldRelayPerformance performance, CancellationToken token)
    {
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.UpdatePerformanceSql,
                performance,
                cancellationToken: token));

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.RemoveAllAthletesForRelayPerformanceSql,
                new { PerformanceId = performance.Id },
                cancellationToken: token));
        
        foreach (var athleteId in performance.AthleteIds)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    FieldRelayPerformanceQueries.CreateAthleteForRelayPerformanceSql,
                    new { Id = Guid.NewGuid(), AthleteId = athleteId, PerformanceId = performance.Id, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
                    cancellationToken: token
                ));
        }

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(FieldRelayPerformance performance, CancellationToken token)
    {
        performance.Deleted = true;
        performance.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.DeletePerformanceSql,
                performance,
                cancellationToken: token));

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.DeleteAllAthletesForRelayPerformanceSql,
                new { PerformanceId = performance.Id },
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
}