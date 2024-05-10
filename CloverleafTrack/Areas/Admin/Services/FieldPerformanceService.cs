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
    public Task CalculateRecordsAsync(CancellationToken token);
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
        
        var orderedPerformances = Performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();
        var eventId = performance.EventId;
        var athleteId = performance.AthleteId;
        var seasonId = performance.Meet.SeasonId;
        var schoolRecord = orderedPerformances.FirstOrDefault(x => x.EventId == eventId);
        var athleteBest = orderedPerformances.FirstOrDefault(x => x.AthleteId == athleteId && x.EventId == eventId);
        var athleteSeasonBest = orderedPerformances.FirstOrDefault(x => x.AthleteId == athleteId && x.Meet.SeasonId == seasonId && x.EventId == eventId);

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

    public async Task CalculateRecordsAsync(CancellationToken token)
    {
        var orderedPerformances = Performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();

        foreach (var performance in orderedPerformances)
        {
            var eventId = performance.EventId;
            var athleteId = performance.AthleteId;
            var seasonId = performance.Meet.SeasonId;
            bool updateDatabase = false;
            
            var schoolRecord = orderedPerformances.First(x => x.EventId == eventId);
            if (performance.Id == schoolRecord.Id && !performance.SchoolRecord)
            {
                performance.SchoolRecord = true;
                updateDatabase = true;
            }
            
            var athleteBest = orderedPerformances.First(x => x.AthleteId == athleteId && x.EventId == eventId);
            if (performance.Id == athleteBest.Id && !performance.PersonalBest)
            {
                performance.PersonalBest = true;
                updateDatabase = true;
            }

            var athleteSeasonBest = orderedPerformances.First(x => x.AthleteId == athleteId && x.Meet.SeasonId == seasonId && x.EventId == eventId);
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