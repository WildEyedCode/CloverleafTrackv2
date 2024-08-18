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
    public Task UpdateAsync(FieldRelayPerformance performance, CancellationToken token, bool updateCache = true);
    public Task DeleteAsync(FieldRelayPerformance performance, CancellationToken token);
    public Task ClearAllRecordsAsync(CancellationToken token);
    public Task CalculateRecordsAsync(CancellationToken token);
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
            performance.AthleteIds = performance.Athletes.Select(x => x.Id).ToList();
        }

        Performances = performances;
    }

    public async Task CreateAsync(FieldRelayPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;
        
        var orderedPerformances = Performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();
        var eventId = performance.EventId;
        var seasonId = performance.Meet.SeasonId;
        var schoolRecord = orderedPerformances.FirstOrDefault(x => x.EventId == eventId);
        var athleteBest = orderedPerformances.FirstOrDefault(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.EventId == eventId);
        var athleteSeasonBest = orderedPerformances.FirstOrDefault(x => x.Athletes.Select(y => y.Id).All(performance.AthleteIds.Contains) && x.Meet.SeasonId == seasonId && x.EventId == eventId);

        if (schoolRecord is null)
        {
            performance.SchoolRecord = true;
        }
        else if (performance.Distance > schoolRecord.Distance)
        {
            performance.SchoolRecord = true;
            schoolRecord.SchoolRecord = false;

            await UpdateAsync(schoolRecord, token);
        }
        
        if (athleteBest is null)
        {
            performance.PersonalBest = true;
        }
        else if (performance.Distance > athleteBest.Distance)
        {
            performance.PersonalBest = true;
            athleteBest.PersonalBest = false;

            await UpdateAsync(athleteBest, token);
        }
        
        if (athleteSeasonBest is null)
        {
            performance.SeasonBest = true;
        }
        else if (performance.Distance > athleteSeasonBest.Distance)
        {
            performance.SeasonBest = true;
            athleteSeasonBest.SeasonBest = false;

            await UpdateAsync(athleteSeasonBest, token);
        }

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        foreach (var dbAthleteId in performance.AthleteIds)
        {
            await connection.ExecuteAsync(
                new CommandDefinition(
                    FieldRelayPerformanceQueries.CreateAthleteForRelayPerformanceSql,
                    new { Id = Guid.NewGuid(), AthleteId = dbAthleteId, PerformanceId = performance.Id, DateCreated = DateTime.UtcNow, DateUpdated = DateTime.UtcNow },
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

    public async Task UpdateAsync(FieldRelayPerformance performance, CancellationToken token, bool updateCache = true)
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

        if (updateCache)
        {
            await ReloadAsync(token);
        }
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
    
    public async Task ClearAllRecordsAsync(CancellationToken token)
    {
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayPerformanceQueries.ClearAllRecordsSql,
                cancellationToken: token));

        await ReloadAsync(token);
    }
    
    public async Task CalculateRecordsAsync(CancellationToken token)
    {
        await ClearAllRecordsAsync(token);
        var orderedPerformances = Performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();

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
                await UpdateAsync(performance, token, false);
            }
        }
    }
}