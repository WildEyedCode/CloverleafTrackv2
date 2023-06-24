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

        return new AthleteViewModel(
            athlete, 
            indoorFieldPerformances,
            outdoorFieldPerformances,
            indoorFieldRelayPerformances,
            outdoorFieldRelayPerformances,
            indoorRunningPerformances,
            outdoorRunningPerformances,
            indoorRunningRelayPerformances,
            outdoorRunningRelayPerformances);
    }
}
