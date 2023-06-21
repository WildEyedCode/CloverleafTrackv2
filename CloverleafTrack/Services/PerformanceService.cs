using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface IPerformanceService
{
    public List<FieldPerformance> FieldPerformances { get; }
    public List<FieldPerformance> IndoorFieldPerformances { get; }
    public List<FieldPerformance> OutdoorFieldPerformances { get; }
    public List<FieldRelayPerformance> FieldRelayPerformances { get; }
    public List<FieldRelayPerformance> IndoorFieldRelayPerformances { get; }
    public List<FieldRelayPerformance> OutdoorFieldRelayPerformances { get; }
    public List<RunningPerformance> RunningPerformances { get; }
    public List<RunningPerformance> IndoorRunningPerformances { get; }
    public List<RunningPerformance> OutdoorRunningPerformances { get; }
    public List<RunningRelayPerformance> RunningRelayPerformances { get; }
    public List<RunningRelayPerformance> IndoorRunningRelayPerformances { get; }
    public List<RunningRelayPerformance> OutdoorRunningRelayPerformances { get; }
    public int FieldPerformanceCount { get; }
    public int FieldRelayPerformanceCount { get; }
    public int RunningPerformanceCount { get; }
    public int RunningRelayPerformanceCount { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
    public FieldPerformance? GetBestForEventAndSeason(FieldEvent @event, Season season);
    public FieldRelayPerformance? GetBestForEventAndSeason(FieldRelayEvent @event, Season season);
    public RunningPerformance? GetBestForEventAndSeason(RunningEvent @event, Season season);
    public RunningRelayPerformance? GetBestForEventAndSeason(RunningRelayEvent @event, Season season);
}

public class PerformanceService : IPerformanceService
{
    private readonly IDbConnection connection;

    public PerformanceService(IDbConnection connection)
    {
        this.connection = connection;
        FieldPerformances = new List<FieldPerformance>();
        FieldRelayPerformances = new List<FieldRelayPerformance>();
        RunningPerformances = new List<RunningPerformance>();
        RunningRelayPerformances = new List<RunningRelayPerformance>();
    }
    
    public List<FieldPerformance> FieldPerformances { get; private set; }
    public List<FieldPerformance> IndoorFieldPerformances => FieldPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<FieldPerformance> OutdoorFieldPerformances => FieldPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<FieldRelayPerformance> FieldRelayPerformances { get; private set; }
    public List<FieldRelayPerformance> IndoorFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<FieldRelayPerformance> OutdoorFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<RunningPerformance> RunningPerformances { get; private set; }
    public List<RunningPerformance> IndoorRunningPerformances => RunningPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<RunningPerformance> OutdoorRunningPerformances => RunningPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<RunningRelayPerformance> RunningRelayPerformances { get; private set; }
    public List<RunningRelayPerformance> IndoorRunningRelayPerformances => RunningRelayPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<RunningRelayPerformance> OutdoorRunningRelayPerformances => RunningRelayPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public int FieldPerformanceCount => FieldPerformances.Count;
    public int FieldRelayPerformanceCount => FieldRelayPerformances.Count;
    public int RunningPerformanceCount => RunningPerformances.Count;
    public int RunningRelayPerformanceCount => RunningRelayPerformances.Count;
    public int Count => FieldPerformanceCount + FieldRelayPerformanceCount + RunningPerformanceCount + RunningRelayPerformanceCount;

    public async Task ReloadAsync(CancellationToken token)
    {
        FieldPerformances = await ReloadFieldPerformancesAsync();
        FieldRelayPerformances = await ReloadFieldRelayPerformancesAsync();
        RunningPerformances = await ReloadRunningPerformancesAsync();
        RunningRelayPerformances = await ReloadRunningRelayPerformancesAsync();
    }
    
    public FieldPerformance? GetBestForEventAndSeason(FieldEvent @event, Season season)
    {
        var performances = FieldPerformances.FindAll(x => x.Meet.SeasonId == season.Id && x.EventId == @event.Id);
        return performances.MaxBy(x => x.Distance);
    }
    
    public FieldRelayPerformance? GetBestForEventAndSeason(FieldRelayEvent @event, Season season)
    {
        var performances = FieldRelayPerformances.FindAll(x => x.Meet.SeasonId == season.Id && x.EventId == @event.Id);
        return performances.MaxBy(x => x.Distance);
    }
    
    public RunningPerformance? GetBestForEventAndSeason(RunningEvent @event, Season season)
    {
        var performances = RunningPerformances.FindAll(x => x.Meet.SeasonId == season.Id && x.EventId == @event.Id);
        return performances.MinBy(x => x.Time);
    }
    
    public RunningRelayPerformance? GetBestForEventAndSeason(RunningRelayEvent @event, Season season)
    {
        var performances = RunningRelayPerformances.FindAll(x => x.Meet.SeasonId == season.Id && x.EventId == @event.Id);
        return performances.MinBy(x => x.Time);
    }

    private async Task<List<FieldPerformance>> ReloadFieldPerformancesAsync()
    {
        return (await connection.QueryAsync<FieldPerformance, FieldEvent, Meet, Season, Athlete, FieldPerformance>(PerformanceQueries.SelectAllFieldPerformancesSql,
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
    
    private async Task<List<FieldRelayPerformance>> ReloadFieldRelayPerformancesAsync()
    {
        var performances = (await connection.QueryAsync<FieldRelayPerformance, FieldRelayEvent, Meet, Season, FieldRelayPerformance>(PerformanceQueries.SelectAllFieldRelayPerformancesSql,
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
            .OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches).ToList();

        foreach (var performance in performances)
        {
            var athletes = await connection.QueryAsync<Athlete>(PerformanceQueries.SelectAthletesForFieldRelayPerformanceSql, new { PerformanceId = performance.Id });
            performance.Athletes = athletes.ToList();
        }

        return performances;
    }
    
    private async Task<List<RunningPerformance>> ReloadRunningPerformancesAsync()
    {
        return (await connection.QueryAsync<RunningPerformance, RunningEvent, Meet, Season, Athlete, RunningPerformance>(PerformanceQueries.SelectAllRunningPerformancesSql,
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
    
    private async Task<List<RunningRelayPerformance>> ReloadRunningRelayPerformancesAsync()
    {
        var performances =  (await connection.QueryAsync<RunningRelayPerformance, RunningRelayEvent, Meet, Season, RunningRelayPerformance>(PerformanceQueries.SelectAllRunningRelayPerformancesSql,
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
            .OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();
        
        foreach (var performance in performances)
        {
            var athletes = await connection.QueryAsync<Athlete>(PerformanceQueries.SelectAthletesForRunningRelayPerformanceSql, new { PerformanceId = performance.Id });
            performance.Athletes = athletes.ToList();
        }

        return performances;
    }
}
