using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IRunningPerformanceService
{
    public List<RunningPerformance> Performances { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(RunningPerformance performance, CancellationToken token);
    public List<RunningPerformance> ReadAll();
    public RunningPerformance? ReadById(Guid id);
    public Task UpdateAsync(RunningPerformance performance, CancellationToken token);
    public Task DeleteAsync(RunningPerformance performance, CancellationToken token);
    public Task CalculateRecordsAsync(CancellationToken token);
}

public class RunningPerformanceService : IRunningPerformanceService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public RunningPerformanceService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Performances = new List<RunningPerformance>();
    }
    
    public List<RunningPerformance> Performances { get; private set; }

    public int Count => Performances.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        Performances = (await connection.QueryAsync<RunningPerformance, RunningEvent, Meet, Season, Athlete, RunningPerformance>(RunningPerformanceQueries.AllPerformancesSql,
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
            .OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();
    }

    public async Task CreateAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;
        
        var orderedPerformances = Performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();
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
                RunningPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<RunningPerformance> ReadAll()
    {
        return Performances;
    }

    public RunningPerformance? ReadById(Guid id)
    {
        return Performances.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningPerformanceQueries.UpdatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.Deleted = true;
        performance.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningPerformanceQueries.DeletePerformanceSql,
                performance,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
    
    public async Task CalculateRecordsAsync(CancellationToken token)
    {
        var orderedPerformances = Performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();

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