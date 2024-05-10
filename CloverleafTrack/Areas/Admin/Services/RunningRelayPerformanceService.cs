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
    public Task CalculateRecordsAsync(CancellationToken token);
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
            performance.AthleteIds = performance.Athletes.Select(x => x.Id).ToList();
        }

        Performances = performances;
    }

    public async Task CreateAsync(RunningRelayPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;
        
        var orderedPerformances = Performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();
        var eventId = performance.EventId;
        var seasonId = performance.Meet.SeasonId;
        var schoolRecord = orderedPerformances.FirstOrDefault(x => x.EventId == eventId);
        var athleteBest = orderedPerformances.FirstOrDefault(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.EventId == eventId);
        var athleteSeasonBest = orderedPerformances.FirstOrDefault(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.Meet.SeasonId == seasonId && x.EventId == eventId);

        if (schoolRecord is null)
        {
            performance.SchoolRecord = true;
        }
        else if (performance.Time < schoolRecord.Time)
        {
            performance.SchoolRecord = true;
            schoolRecord.SchoolRecord = false;

            await UpdateAsync(schoolRecord, token);
        }
        
        if (athleteBest is null)
        {
            performance.PersonalBest = true;
        }
        else if (performance.Time < athleteBest.Time)
        {
            performance.PersonalBest = true;
            athleteBest.PersonalBest = false;

            await UpdateAsync(athleteBest, token);
        }
        
        if (athleteSeasonBest is null)
        {
            performance.SeasonBest = true;
        }
        else if (performance.Time < athleteSeasonBest.Time)
        {
            performance.SeasonBest = true;
            athleteSeasonBest.SeasonBest = false;

            await UpdateAsync(athleteSeasonBest, token);
        }

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
    
    public async Task CalculateRecordsAsync(CancellationToken token)
    {
        var orderedPerformances = Performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();

        foreach (var performance in orderedPerformances)
        {
            performance.AthleteIds = performance.Athletes.Select(x => x.Id).ToList();
            var eventId = performance.EventId;
            var seasonId = performance.Meet.SeasonId;
            bool updateDatabase = false;
            
            var schoolRecord = orderedPerformances.First(x => x.EventId == eventId);
            if (performance.Id == schoolRecord.Id && !performance.SchoolRecord)
            {
                performance.SchoolRecord = true;
                updateDatabase = true;
            }
            
            var athleteBest = orderedPerformances.First(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.EventId == eventId);
            if (performance.Id == athleteBest.Id && !performance.PersonalBest)
            {
                performance.PersonalBest = true;
                updateDatabase = true;
            }

            var athleteSeasonBest = orderedPerformances.First(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.Meet.SeasonId == seasonId && x.EventId == eventId);
            if (performance.Id == athleteSeasonBest.Id && !performance.SeasonBest)
            {
                performance.SeasonBest = true;
                updateDatabase = true;
            }

            if (updateDatabase)
            {
                await UpdateAsync(performance, token);
            }
        }
    }
}