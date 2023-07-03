using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Microsoft.Extensions.Options;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface IAthleteService
{
    public List<Athlete> Athletes { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public List<Athlete> CurrentAthletes(bool gender);
    public List<Athlete> PastAthletes(bool gender);
    public Athlete? GetAthleteByUrlName(string urlName);
    public RosterViewModel GetRoster();
    public AthleteViewModel GetAthlete(string urlName);
}

public class AthleteService : IAthleteService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;
    private readonly IPerformanceService performanceService;

    public AthleteService(IDbConnection connection, IOptions<CloverleafTrackOptions> options, IPerformanceService performanceService)
    {
        this.connection = connection;
        this.options = options.Value;
        this.performanceService = performanceService;
        Athletes = new List<Athlete>();
    }
    
    public List<Athlete> Athletes { get; private set; }
    public int Count => Athletes.Count;
    
    public async Task ReloadAsync(CancellationToken token)
    {
        Athletes = (await connection.QueryAsync<Athlete>(AthleteQueries.SelectAllAthletesSql)).ToList();
    }

    public List<Athlete> CurrentAthletes(bool gender)
    {
        return Athletes
            .Where(x => x.GraduationYear >= options.GraduationYear && x.GraduationYear <= options.GraduationYear + 3 && x.Gender == gender)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public List<Athlete> PastAthletes(bool gender)
    {
        return Athletes
            .Where(x => x.GraduationYear < options.GraduationYear && x.Gender == gender)
            .OrderByDescending(x => x.GraduationYear)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public Athlete? GetAthleteByUrlName(string urlName)
    {
        return Athletes.FirstOrDefault(x => x.UrlName == urlName);
    }

    public RosterViewModel GetRoster()
    {
        return new RosterViewModel(CurrentAthletes(false), CurrentAthletes(true), PastAthletes(false), PastAthletes(true));
    }

    public AthleteViewModel GetAthlete(string urlName)
    {
        var athlete = GetAthleteByUrlName(urlName);
        var indoorFieldPerformances = performanceService.GetFieldPerformancesByAthlete(athlete, Environment.Indoor);
        var outdoorFieldPerformances = performanceService.GetFieldPerformancesByAthlete(athlete, Environment.Outdoor);
        var indoorFieldRelayPerformances = performanceService.GetFieldRelayPerformancesByAthlete(athlete, Environment.Indoor);
        var outdoorFieldRelayPerformances = performanceService.GetFieldRelayPerformancesByAthlete(athlete, Environment.Outdoor);
        var indoorRunningPerformances = performanceService.GetRunningPerformancesByAthlete(athlete, Environment.Indoor);
        var outdoorRunningPerformances = performanceService.GetRunningPerformancesByAthlete(athlete, Environment.Outdoor);
        var indoorRunningRelayPerformances = performanceService.GetRunningRelayPerformancesByAthlete(athlete, Environment.Indoor);
        var outdoorRunningRelayPerformances = performanceService.GetRunningRelayPerformancesByAthlete(athlete, Environment.Outdoor);

        var indoorFieldSeasonBests = performanceService.GetFieldPerformanceSeasonBestsByAthlete(athlete, Environment.Indoor);
        var outdoorFieldSeasonBests = performanceService.GetFieldPerformanceSeasonBestsByAthlete(athlete, Environment.Outdoor);
        var indoorFieldRelaySeasonBests = performanceService.GetFieldRelayPerformanceSeasonBestsByAthlete(athlete, Environment.Indoor);
        var outdoorFieldRelaySeasonBests = performanceService.GetFieldRelayPerformanceSeasonBestsByAthlete(athlete, Environment.Outdoor);
        var indoorRunningSeasonBests = performanceService.GetRunningPerformanceSeasonBestsByAthlete(athlete, Environment.Indoor);
        var outdoorRunningSeasonBests = performanceService.GetRunningPerformanceSeasonBestsByAthlete(athlete, Environment.Outdoor);
        var indoorRunningRelaySeasonBests = performanceService.GetRunningRelayPerformanceSeasonBestsByAthlete(athlete, Environment.Indoor);
        var outdoorRunningRelaySeasonBests = performanceService.GetRunningRelayPerformanceSeasonBestsByAthlete(athlete, Environment.Outdoor);

        var seasons = indoorFieldSeasonBests.Select(x => x.Season)
            .Union(outdoorFieldSeasonBests.Select(x => x.Season))
            .Union(indoorFieldRelaySeasonBests.Select(x => x.Season))
            .Union(outdoorFieldRelaySeasonBests.Select(x => x.Season))
            .Union(indoorRunningSeasonBests.Select(x => x.Season))
            .Union(outdoorRunningSeasonBests.Select(x => x.Season))
            .Union(indoorRunningRelaySeasonBests.Select(x => x.Season))
            .Union(outdoorRunningRelaySeasonBests.Select(x => x.Season))
            .OrderByDescending(x => x.Name)
            .ToList();
        
        var indoorFieldBests = performanceService.GetFieldPerformanceBestsByAthlete(athlete, Environment.Indoor);
        var outdoorFieldBests = performanceService.GetFieldPerformanceBestsByAthlete(athlete, Environment.Outdoor);
        var indoorFieldRelayBests = performanceService.GetFieldRelayPerformanceBestsByAthlete(athlete, Environment.Indoor);
        var outdoorFieldRelayBests = performanceService.GetFieldRelayPerformanceBestsByAthlete(athlete, Environment.Outdoor);
        var indoorRunningBests = performanceService.GetRunningPerformanceBestsByAthlete(athlete, Environment.Indoor);
        var outdoorRunningBests = performanceService.GetRunningPerformanceBestsByAthlete(athlete, Environment.Outdoor);
        var indoorRunningRelayBests = performanceService.GetRunningRelayPerformanceBestsByAthlete(athlete, Environment.Indoor);
        var outdoorRunningRelayBests = performanceService.GetRunningRelayPerformanceBestsByAthlete(athlete, Environment.Outdoor);

        var hasIndoorPerformances = indoorFieldPerformances.Any() || indoorFieldRelayPerformances.Any() || indoorRunningPerformances.Any() || indoorRunningRelayPerformances.Any();
        var hasOutdoorPerformances = outdoorFieldPerformances.Any() || outdoorFieldRelayPerformances.Any() || outdoorRunningPerformances.Any() || outdoorRunningRelayPerformances.Any();
        var hasIndoorLifetimeBests = indoorFieldBests.Any() || indoorFieldRelayBests.Any() || indoorRunningBests.Any() || indoorRunningRelayBests.Any();
        var hasOutdoorLifetimeBests = outdoorFieldBests.Any() || outdoorFieldRelayBests.Any() || outdoorRunningBests.Any() || outdoorRunningRelayBests.Any();
        var hasLifetimeBests = hasIndoorLifetimeBests || hasOutdoorLifetimeBests;
        var hasIndoorSeasonBests = indoorFieldSeasonBests.Any() || indoorFieldRelaySeasonBests.Any() || indoorRunningSeasonBests.Any() || indoorRunningRelaySeasonBests.Any();
        var hasOutdoorSeasonBests = outdoorFieldSeasonBests.Any() || outdoorFieldRelaySeasonBests.Any() || outdoorRunningSeasonBests.Any() || outdoorRunningRelaySeasonBests.Any();
        var hasSeasonBests = hasIndoorSeasonBests || hasOutdoorSeasonBests;
        
        return new AthleteViewModel(
            athlete, 
            indoorFieldPerformances,
            outdoorFieldPerformances,
            indoorFieldRelayPerformances,
            outdoorFieldRelayPerformances,
            indoorRunningPerformances,
            outdoorRunningPerformances,
            indoorRunningRelayPerformances,
            outdoorRunningRelayPerformances,
            seasons,
            indoorFieldSeasonBests,
            outdoorFieldSeasonBests,
            indoorFieldRelaySeasonBests,
            outdoorFieldRelaySeasonBests,
            indoorRunningSeasonBests,
            outdoorRunningSeasonBests,
            indoorRunningRelaySeasonBests,
            outdoorRunningRelaySeasonBests,
            indoorFieldBests,
            outdoorFieldBests,
            indoorFieldRelayBests,
            outdoorFieldRelayBests,
            indoorRunningBests,
            outdoorRunningBests,
            indoorRunningRelayBests,
            outdoorRunningRelayBests,
            hasIndoorPerformances,
            hasOutdoorPerformances,
            hasIndoorLifetimeBests,
            hasOutdoorLifetimeBests,
            hasLifetimeBests,
            hasIndoorSeasonBests,
            hasOutdoorSeasonBests,
            hasSeasonBests);
    }
}
