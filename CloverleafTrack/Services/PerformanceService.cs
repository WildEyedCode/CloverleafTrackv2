using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface IPerformanceService
{
    public List<FieldPerformance> FieldPerformances { get; }
    public List<FieldPerformance> BoysFieldPerformances { get; }
    public List<FieldPerformance> GirlsFieldPerformances { get; }
    public List<FieldPerformance> IndoorFieldPerformances { get; }
    public List<FieldPerformance> OutdoorFieldPerformances { get; }
    public List<FieldRelayPerformance> FieldRelayPerformances { get; }
    public List<FieldRelayPerformance> BoysFieldRelayPerformances { get; }
    public List<FieldRelayPerformance> GirlsFieldRelayPerformances { get; }
    public List<FieldRelayPerformance> IndoorFieldRelayPerformances { get; }
    public List<FieldRelayPerformance> OutdoorFieldRelayPerformances { get; }
    public List<RunningPerformance> RunningPerformances { get; }
    public List<RunningPerformance> BoysRunningPerformances { get; }
    public List<RunningPerformance> GirlsRunningPerformances { get; }
    public List<RunningPerformance> IndoorRunningPerformances { get; }
    public List<RunningPerformance> OutdoorRunningPerformances { get; }
    public List<RunningRelayPerformance> RunningRelayPerformances { get; }
    public List<RunningRelayPerformance> BoysRunningRelayPerformances { get; }
    public List<RunningRelayPerformance> GirlsRunningRelayPerformances { get; }
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
    public List<FieldEventPerformancesViewModel> GetFieldPerformancesByAthlete(Athlete? athlete, Environment environment);
    public List<FieldRelayEventPerformancesViewModel> GetFieldRelayPerformancesByAthlete(Athlete? athlete, Environment environment);
    public List<RunningEventPerformancesViewModel> GetRunningPerformancesByAthlete(Athlete? athlete, Environment environment);
    public List<RunningRelayEventPerformancesViewModel> GetRunningRelayPerformancesByAthlete(Athlete? athlete, Environment environment);
    List<FieldEventSeasonViewModel> GetFieldPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment);
    List<FieldRelayEventSeasonViewModel> GetFieldRelayPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment);
    List<RunningEventSeasonViewModel> GetRunningPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment);
    List<RunningRelayEventSeasonViewModel> GetRunningRelayPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment);
    public List<FieldEventBestViewModel> GetFieldPerformanceBestsByAthlete(Athlete? athlete, Environment environment);
    public List<FieldRelayEventBestViewModel> GetFieldRelayPerformanceBestsByAthlete(Athlete? athlete, Environment environment);
    public List<RunningEventBestViewModel> GetRunningPerformanceBestsByAthlete(Athlete? athlete, Environment environment);
    public List<RunningRelayEventBestViewModel> GetRunningRelayPerformanceBestsByAthlete(Athlete? athlete, Environment environment);
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
    public List<FieldPerformance> BoysFieldPerformances => FieldPerformances.FindAll(x => x.Event.Gender == Gender.Male);
    public List<FieldPerformance> GirlsFieldPerformances => FieldPerformances.FindAll(x => x.Event.Gender == Gender.Female);
    public List<FieldPerformance> IndoorFieldPerformances => FieldPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<FieldPerformance> OutdoorFieldPerformances => FieldPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<FieldRelayPerformance> FieldRelayPerformances { get; private set; }
    public List<FieldRelayPerformance> BoysFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Gender == Gender.Male);
    public List<FieldRelayPerformance> GirlsFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Gender == Gender.Female);
    public List<FieldRelayPerformance> IndoorFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<FieldRelayPerformance> OutdoorFieldRelayPerformances => FieldRelayPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<RunningPerformance> RunningPerformances { get; private set; }
    public List<RunningPerformance> BoysRunningPerformances => RunningPerformances.FindAll(x => x.Event.Gender == Gender.Male);
    public List<RunningPerformance> GirlsRunningPerformances => RunningPerformances.FindAll(x => x.Event.Gender == Gender.Female);
    public List<RunningPerformance> IndoorRunningPerformances => RunningPerformances.FindAll(x => x.Event.Environment == Environment.Indoor);
    public List<RunningPerformance> OutdoorRunningPerformances => RunningPerformances.FindAll(x => x.Event.Environment == Environment.Outdoor);
    public List<RunningRelayPerformance> RunningRelayPerformances { get; private set; }
    public List<RunningRelayPerformance> BoysRunningRelayPerformances => RunningRelayPerformances.FindAll(x => x.Event.Gender == Gender.Male);
    public List<RunningRelayPerformance> GirlsRunningRelayPerformances => RunningRelayPerformances.FindAll(x => x.Event.Gender == Gender.Female);
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
    
    public List<FieldEventPerformancesViewModel> GetFieldPerformancesByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldEventPerformancesViewModel>().ToList();
        }

        var output = new List<FieldEventPerformancesViewModel>();
        var performances = FieldPerformances.FindAll(x => x.Event.Environment == environment && x.AthleteId == athlete.Id);
        foreach (var performance in performances)
        {
            var existing = output.Find(x => x.Event.Id == performance.EventId);
            if (existing != null)
            {
                existing.Performances.Add(performance);
            }
            else
            {
                output.Add(new FieldEventPerformancesViewModel(performance.Event, new List<FieldPerformance> { performance }));
            }
        }

        return output;
    }
    
    public List<FieldRelayEventPerformancesViewModel> GetFieldRelayPerformancesByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldRelayEventPerformancesViewModel>().ToList();
        }

        var output = new List<FieldRelayEventPerformancesViewModel>();
        var performances = FieldRelayPerformances.FindAll(x => x.Event.Environment == environment && x.Athletes.Any(y => y.Id == athlete.Id));
        foreach (var performance in performances)
        {
            var existing = output.Find(x => x.Event.Id == performance.EventId);
            if (existing != null)
            {
                existing.Performances.Add(performance);
            }
            else
            {
                output.Add(new FieldRelayEventPerformancesViewModel(performance.Event, new List<FieldRelayPerformance> { performance }));
            }
        }

        return output;
    }
    
    public List<RunningEventPerformancesViewModel> GetRunningPerformancesByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningEventPerformancesViewModel>().ToList();
        }

        var output = new List<RunningEventPerformancesViewModel>();
        var performances = RunningPerformances.FindAll(x => x.Event.Environment == environment && x.AthleteId == athlete.Id);
        foreach (var performance in performances)
        {
            var existing = output.Find(x => x.Event.Id == performance.EventId);
            if (existing != null)
            {
                existing.Performances.Add(performance);
            }
            else
            {
                output.Add(new RunningEventPerformancesViewModel(performance.Event, new List<RunningPerformance> { performance }));
            }
        }

        return output;
    }
    
    public List<RunningRelayEventPerformancesViewModel> GetRunningRelayPerformancesByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningRelayEventPerformancesViewModel>().ToList();
        }

        var output = new List<RunningRelayEventPerformancesViewModel>();
        var performances = RunningRelayPerformances.FindAll(x => x.Event.Environment == environment && x.Athletes.Any(y => y.Id == athlete.Id));
        foreach (var performance in performances)
        {
            var existing = output.Find(x => x.Event.Id == performance.EventId);
            if (existing != null)
            {
                existing.Performances.Add(performance);
            }
            else
            {
                output.Add(new RunningRelayEventPerformancesViewModel(performance.Event, new List<RunningRelayPerformance> { performance }));
            }
        }

        return output;
    }
    
    public List<FieldEventSeasonViewModel> GetFieldPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldEventSeasonViewModel>().ToList();
        }

        var output = new List<FieldEventSeasonViewModel>();
        var performances = FieldPerformances.FindAll(x => x.Meet.Environment == environment && x.AthleteId == athlete.Id);
        var group = performances.GroupBy(x => x.Meet.Season);
        foreach (var grouping in group)
        {
            var eventGroup = grouping.GroupBy(x => x.Event);
            foreach (var eventGrouping in eventGroup)
            {
                var best = eventGrouping.Select(x => x).MaxBy(x => x.Distance);
                if (best != null)
                {
                    var existing = output.FirstOrDefault(x => x.Season.Id == grouping.Key.Id);
                    if (existing != null)
                    {
                        existing.Performances.Add(new FieldEventBestViewModel(best.Event, best));
                    }
                    else
                    {
                        output.Add(new FieldEventSeasonViewModel(grouping.Key, new List<FieldEventBestViewModel> { new(best.Event, best)}));
                    }
                }  
            }
        }
        
        return output;
    }
    
    public List<FieldRelayEventSeasonViewModel> GetFieldRelayPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldRelayEventSeasonViewModel>().ToList();
        }
        
        var output = new List<FieldRelayEventSeasonViewModel>();
        var performances = FieldRelayPerformances.FindAll(x => x.Meet.Environment == environment && x.Athletes.Any(y => y.Id == athlete.Id));
        var group = performances.GroupBy(x => x.Meet.Season);
        foreach (var grouping in group)
        {
            var eventGroup = grouping.GroupBy(x => x.Event);
            foreach (var eventGrouping in eventGroup)
            {
                var best = eventGrouping.Select(x => x).MaxBy(x => x.Distance);
                if (best != null)
                {
                    var existing = output.FirstOrDefault(x => x.Season.Id == grouping.Key.Id);
                    if (existing != null)
                    {
                        existing.Performances.Add(new FieldRelayEventBestViewModel(best.Event, best));
                    }
                    else
                    {
                        output.Add(new FieldRelayEventSeasonViewModel(grouping.Key, new List<FieldRelayEventBestViewModel> { new(best.Event, best)}));
                    }
                }
            }
        }
        
        return output;
    }
    
    public List<RunningEventSeasonViewModel> GetRunningPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningEventSeasonViewModel>().ToList();
        }
        
        var output = new List<RunningEventSeasonViewModel>();
        var performances = RunningPerformances.FindAll(x => x.Meet.Environment == environment && x.AthleteId == athlete.Id);
        var group = performances.GroupBy(x => x.Meet.Season);
        foreach (var grouping in group)
        {
            var eventGroup = grouping.GroupBy(x => x.Event);
            foreach (var eventGrouping in eventGroup)
            {
                var best = eventGrouping.Select(x => x).MinBy(x => x.Time);
                if (best != null)
                {
                    var existing = output.FirstOrDefault(x => x.Season.Id == grouping.Key.Id);
                    if (existing != null)
                    {
                        existing.Performances.Add(new RunningEventBestViewModel(best.Event, best));
                    }
                    else
                    {
                        output.Add(new RunningEventSeasonViewModel(grouping.Key, new List<RunningEventBestViewModel> { new(best.Event, best)}));
                    }
                }
            }
        }
        
        return output;
    }
    
    public List<RunningRelayEventSeasonViewModel> GetRunningRelayPerformanceSeasonBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningRelayEventSeasonViewModel>().ToList();
        }
        
        var output = new List<RunningRelayEventSeasonViewModel>();
        var performances = RunningRelayPerformances.FindAll(x => x.Meet.Environment == environment && x.Athletes.Any(y => y.Id == athlete.Id));
        var group = performances.GroupBy(x => x.Meet.Season);
        foreach (var grouping in group)
        {
            var eventGroup = grouping.GroupBy(x => x.Event);
            foreach (var eventGrouping in eventGroup)
            {
                var best = eventGrouping.Select(x => x).MinBy(x => x.Time);
                if (best != null)
                {
                    var existing = output.FirstOrDefault(x => x.Season.Id == grouping.Key.Id);
                    if (existing != null)
                    {
                        existing.Performances.Add(new RunningRelayEventBestViewModel(best.Event, best));
                    }
                    else
                    {
                        output.Add(new RunningRelayEventSeasonViewModel(grouping.Key, new List<RunningRelayEventBestViewModel> { new(best.Event, best)}));
                    }
                }
            }
        }
        
        return output;
    }

    public List<FieldEventBestViewModel> GetFieldPerformanceBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldEventBestViewModel>().ToList();
        }

        var output = new List<FieldEventBestViewModel>();
        var allPerformances = GetFieldPerformancesByAthlete(athlete, environment);
        foreach (var eventPerformance in allPerformances)
        {
            var best = eventPerformance.Performances.MaxBy(x => x.Distance);
            if (best != null)
            {
                var vm = new FieldEventBestViewModel(eventPerformance.Event, best);
                output.Add(vm);
            }
        }

        return output;
    }
    
    public List<FieldRelayEventBestViewModel> GetFieldRelayPerformanceBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<FieldRelayEventBestViewModel>().ToList();
        }
        
        var output = new List<FieldRelayEventBestViewModel>();
        var allPerformances = GetFieldRelayPerformancesByAthlete(athlete, environment);
        foreach (var eventPerformance in allPerformances)
        {
            var best = eventPerformance.Performances.MaxBy(x => x.Distance);
            if (best != null)
            {
                var vm = new FieldRelayEventBestViewModel(eventPerformance.Event, best);
                output.Add(vm);
            }

        }

        return output;
    }
    
    public List<RunningEventBestViewModel> GetRunningPerformanceBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningEventBestViewModel>().ToList();
        }
        
        var output = new List<RunningEventBestViewModel>();
        var allPerformances = GetRunningPerformancesByAthlete(athlete, environment);
        foreach (var eventPerformance in allPerformances)
        {
            var best = eventPerformance.Performances.MinBy(x => x.Time);
            if (best != null)
            {
                var vm = new RunningEventBestViewModel(eventPerformance.Event, best);
                output.Add(vm);
            }

        }

        return output;
    }
    
    public List<RunningRelayEventBestViewModel> GetRunningRelayPerformanceBestsByAthlete(Athlete? athlete, Environment environment)
    {
        if (athlete == null)
        {
            return Enumerable.Empty<RunningRelayEventBestViewModel>().ToList();
        }
        
        var output = new List<RunningRelayEventBestViewModel>();
        var allPerformances = GetRunningRelayPerformancesByAthlete(athlete, environment);
        foreach (var eventPerformance in allPerformances)
        {
            var best = eventPerformance.Performances.MinBy(x => x.Time);
            if (best != null)
            {
                var vm = new RunningRelayEventBestViewModel(eventPerformance.Event, best);
                output.Add(vm);
            }

        }

        return output;
    }

    private async Task<List<FieldPerformance>> ReloadFieldPerformancesAsync()
    {
        return (await connection.QueryAsync<FieldPerformance, FieldEvent, Meet, Season, Athlete, FieldPerformance>(PerformanceQueries.AllFieldPerformancesSql,
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
        var performances = (await connection.QueryAsync<FieldRelayPerformance, FieldRelayEvent, Meet, Season, FieldRelayPerformance>(PerformanceQueries.AllFieldRelayPerformancesSql,
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
            var athletes = await connection.QueryAsync<Athlete>(PerformanceQueries.AthletesForFieldRelayPerformanceSql, new { PerformanceId = performance.Id });
            performance.Athletes = athletes.ToList();
        }

        return performances;
    }
    
    private async Task<List<RunningPerformance>> ReloadRunningPerformancesAsync()
    {
        return (await connection.QueryAsync<RunningPerformance, RunningEvent, Meet, Season, Athlete, RunningPerformance>(PerformanceQueries.AllRunningPerformancesSql,
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
        var performances =  (await connection.QueryAsync<RunningRelayPerformance, RunningRelayEvent, Meet, Season, RunningRelayPerformance>(PerformanceQueries.AllRunningRelayPerformancesSql,
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
            var athletes = await connection.QueryAsync<Athlete>(PerformanceQueries.AthletesForRunningRelayPerformanceSql, new { PerformanceId = performance.Id });
            performance.Athletes = athletes.ToList();
        }

        return performances;
    }
}
