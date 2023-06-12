using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

namespace CloverleafTrack.Services;

public interface IPerformanceService
{
    public List<FieldPerformance> FieldPerformances { get; }
    public List<FieldRelayPerformance> FieldRelayPerformances { get; }
    public List<RunningPerformance> RunningPerformances { get; }
    public List<RunningRelayPerformance> RunningRelayPerformances { get; }
    public int FieldPerformanceCount { get; }
    public int FieldRelayPerformanceCount { get; }
    public int RunningPerformanceCount { get; }
    public int RunningRelayPerformanceCount { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
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
    public List<FieldRelayPerformance> FieldRelayPerformances { get; private set; }
    public List<RunningPerformance> RunningPerformances { get; private set; }
    public List<RunningRelayPerformance> RunningRelayPerformances { get; private set; }
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
