using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface ISeasonService
{
    public List<Season> Seasons { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
    MeetsViewModel GetSeasonsWithMeets();
    SeasonsViewModel GetAllSeasons();
    SeasonDetailViewModel? GetSeason(string seasonName);
}

public class SeasonService : ISeasonService
{
    private readonly IDbConnection connection;
    private readonly IEventService eventService;
    private readonly IPerformanceService performanceService;

    public SeasonService(IDbConnection connection, IEventService eventService, IPerformanceService performanceService)
    {
        this.connection = connection;
        this.eventService = eventService;
        this.performanceService = performanceService;
        Seasons = new List<Season>();
    }
    
    public List<Season> Seasons { get; private set; }
    public int Count => Seasons.Count;
    public async Task ReloadAsync(CancellationToken token)
    {
        var seasons = (await connection.QueryAsync<Season, Meet, Season>(SeasonQueries.SelectAllSeasonsSql,
            (season, meet) =>
            {
                season.Meets.Add(meet);
                return season;
            },
            splitOn: "MeetId"))
            .OrderByDescending(x => x.Name).ToList();

        Seasons = seasons.GroupBy(x => x.Id).Select(x =>
        {
            var groupedSeason = x.First();
            groupedSeason.Meets = x.Select(s => s.Meets.Single()).OrderByDescending(y => y.Date).ToList();
            return groupedSeason;
        }).ToList();
    }
    public MeetsViewModel GetSeasonsWithMeets()
    {
        return new MeetsViewModel(Seasons.Select(x => new SeasonMeetViewModel(x, x.OutdoorMeets, x.IndoorMeets)).ToList());
    }

    public SeasonsViewModel GetAllSeasons()
    {
        var seasons = new List<SeasonDetailViewModel>();
        foreach (var season in Seasons)
        {
            var vm = new SeasonDetailViewModel(
                season,
                Enumerable.Empty<BestViewModel>().ToList(),
                Enumerable.Empty<BestViewModel>().ToList(),
                Enumerable.Empty<BestRelayViewModel>().ToList(),
                Enumerable.Empty<BestRelayViewModel>().ToList(),
                Enumerable.Empty<BestViewModel>().ToList(),
                Enumerable.Empty<BestViewModel>().ToList(),
                Enumerable.Empty<BestRelayViewModel>().ToList(),
                Enumerable.Empty<BestRelayViewModel>().ToList(),
                season.IndoorMeets,
                season.OutdoorMeets);

            seasons.Add(vm);
        }

        return new SeasonsViewModel(seasons);
    }

    public SeasonDetailViewModel? GetSeason(string seasonName)
    {
        var season = Seasons.FirstOrDefault(x => x.Name == seasonName);
        if (season == null)
        {
            return null;
        }
        
        var indoorFieldPerformances = GetFieldBests(season, Environment.Indoor);
        var outdoorFieldPerformances = GetFieldBests(season, Environment.Outdoor);
        var indoorFieldRelayPerformances = GetFieldRelayBests(season, Environment.Indoor);
        var outdoorFieldRelayPerformances = GetFieldRelayBests(season, Environment.Outdoor);
        var indoorRunningPerformances = GetRunningBests(season, Environment.Indoor);
        var outdoorRunningPerformances = GetRunningBests(season, Environment.Outdoor);
        var indoorRunningRelayPerformances = GetRunningRelayBests(season, Environment.Indoor);
        var outdoorRunningRelayPerformances = GetRunningRelayBests(season, Environment.Outdoor);
        var indoorMeets = season.IndoorMeets;
        var outdoorMeets = season.OutdoorMeets;

        return new SeasonDetailViewModel(
            season,
            indoorFieldPerformances,
            outdoorFieldPerformances,
            indoorFieldRelayPerformances,
            outdoorFieldRelayPerformances,
            indoorRunningPerformances,
            outdoorRunningPerformances,
            indoorRunningRelayPerformances,
            outdoorRunningRelayPerformances,
            indoorMeets,
            outdoorMeets);
    }

    private List<BestViewModel> GetFieldBests(Season season, Environment environment)
    {
        var boysPerformances = GetGenderFieldBests(season, environment, Gender.Male);
        var girlsPerformances = GetGenderFieldBests(season, environment, Gender.Female);
        return GetCombinedBests(boysPerformances, girlsPerformances);
    }

    private List<BestViewModel> GetGenderFieldBests(Season season, Environment environment, Gender gender)
    {
        var bests = new List<BestViewModel>();
        var events = eventService.FieldEvents.FindAll(x => x.Environment == environment && x.Gender == gender);
        foreach (var @event in events)
        {
            var performance = performanceService.GetBestForEventAndSeason(@event, season);
            if (performance != null)
            {
                switch (gender)
                {
                    case Gender.Male:
                    {
                        var best = new BestViewModel(
                            performance.Athlete.Name,
                            performance.Athlete.UrlName,
                            performance.Distance.ToString(),
                            performance.PersonalBest,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            false,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.Female:
                    {
                        var best = new BestViewModel(
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            false,
                            performance.Athlete.Name,
                            performance.Athlete.UrlName,
                            performance.Distance.ToString(),
                            performance.PersonalBest,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
                }
            }
        }

        return bests;
    }

    private List<BestRelayViewModel> GetFieldRelayBests(Season season, Environment environment)
    {
        var boysPerformances = GetGenderFieldRelayBests(season, environment, Gender.Male);
        var girlsPerformances = GetGenderFieldRelayBests(season, environment, Gender.Female);
        return GetCombinedRelayBests(boysPerformances, girlsPerformances);
    }
    
    private List<BestRelayViewModel> GetGenderFieldRelayBests(Season season, Environment environment, Gender gender)
    {
        var bests = new List<BestRelayViewModel>();
        var events = eventService.FieldRelayEvents.FindAll(x => x.Environment == environment && x.Gender == gender);
        foreach (var @event in events)
        {
            var performance = performanceService.GetBestForEventAndSeason(@event, season);
            if (performance != null)
            {
                switch (gender)
                {
                    case Gender.Male:
                    {
                        var best = new BestRelayViewModel(
                            performance.Athletes.Select(x => x.Name).ToList(),
                            performance.Athletes.Select(x => x.UrlName).ToList(),
                            performance.Distance.ToString(),
                            performance.PersonalBest,
                            Enumerable.Empty<string>().ToList(),
                            Enumerable.Empty<string>().ToList(),
                            string.Empty,
                            false,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.Female:
                    {
                        var best = new BestRelayViewModel(
                            Enumerable.Empty<string>().ToList(),
                            Enumerable.Empty<string>().ToList(),
                            string.Empty,
                            false,
                            performance.Athletes.Select(x => x.Name).ToList(),
                            performance.Athletes.Select(x => x.UrlName).ToList(),
                            performance.Distance.ToString(),
                            performance.PersonalBest,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
                }
            }
        }

        return bests;
    }

    private List<BestViewModel> GetRunningBests(Season season, Environment environment)
    {
        var boysPerformances = GetGenderRunningBests(season, environment, Gender.Male);
        var girlsPerformances = GetGenderRunningBests(season, environment, Gender.Female);
        return GetCombinedBests(boysPerformances, girlsPerformances);
    }

    private List<BestViewModel> GetGenderRunningBests(Season season, Environment environment, Gender gender)
    {
        var bests = new List<BestViewModel>();
        var events = eventService.RunningEvents.FindAll(x => x.Environment == environment && x.Gender == gender);
        foreach (var @event in events)
        {
            var performance = performanceService.GetBestForEventAndSeason(@event, season);
            if (performance != null)
            {
                switch (gender)
                {
                    case Gender.Male:
                    {
                        var best = new BestViewModel(
                            performance.Athlete.Name,
                            performance.Athlete.UrlName,
                            performance.Time.ToString(),
                            performance.PersonalBest,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            false,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.Female:
                    {
                        var best = new BestViewModel(
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            false,
                            performance.Athlete.Name,
                            performance.Athlete.UrlName,
                            performance.Time.ToString(),
                            performance.PersonalBest,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
                }
            }
        }

        return bests;
    }

    private List<BestRelayViewModel> GetRunningRelayBests(Season season, Environment environment)
    {
        var boysPerformances = GetGenderRunningRelayBests(season, environment, Gender.Male);
        var girlsPerformances = GetGenderRunningRelayBests(season, environment, Gender.Female);
        return GetCombinedRelayBests(boysPerformances, girlsPerformances);
    }
    
    private List<BestRelayViewModel> GetGenderRunningRelayBests(Season season, Environment environment, Gender gender)
    {
        var bests = new List<BestRelayViewModel>();
        var events = eventService.RunningRelayEvents.FindAll(x => x.Environment == environment && x.Gender == gender);
        foreach (var @event in events)
        {
            var performance = performanceService.GetBestForEventAndSeason(@event, season);
            if (performance != null)
            {
                switch (gender)
                {
                    case Gender.Male:
                    {
                        var best = new BestRelayViewModel(
                            performance.Athletes.Select(x => x.Name).ToList(),
                            performance.Athletes.Select(x => x.UrlName).ToList(),
                            performance.Time.ToString(),
                            performance.PersonalBest,
                            Enumerable.Empty<string>().ToList(),
                            Enumerable.Empty<string>().ToList(),
                            string.Empty,
                            false,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.Female:
                    {
                        var best = new BestRelayViewModel(
                            Enumerable.Empty<string>().ToList(),
                            Enumerable.Empty<string>().ToList(),
                            string.Empty,
                            false,
                            performance.Athletes.Select(x => x.Name).ToList(),
                            performance.Athletes.Select(x => x.UrlName).ToList(),
                            performance.Time.ToString(),
                            performance.PersonalBest,
                            @event.Name,
                            @event.SortOrder,
                            environment);

                        bests.Add(best);
                        break;
                    }
                    case Gender.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(gender), gender, null);
                }
            }
        }

        return bests;
    }

    private List<BestViewModel> GetCombinedBests(List<BestViewModel> boysPerformances, List<BestViewModel> girlsPerformances)
    {
        var combined = new List<BestViewModel>();
        
        var eventNames = boysPerformances.Select(x => x.EventName).Union(girlsPerformances.Select(x => x.EventName));
        foreach (var eventName in eventNames)
        {
            var boysPerformance = boysPerformances.SingleOrDefault(x => x.EventName == eventName);
            var girlsPerformance = girlsPerformances.SingleOrDefault(x => x.EventName == eventName);

            var boysAthleteName = boysPerformance != null ? boysPerformance.BoysAthleteName : string.Empty;
            var boysAthleteUrlName = boysPerformance != null ? boysPerformance.BoysAthleteUrlName : string.Empty;
            var boysPerformanceString = boysPerformance != null ? boysPerformance.BoysPerformance : string.Empty;
            var boysPersonalBest = boysPerformance?.BoysPersonalBest ?? false;
            var boysEventSortOrder = boysPerformance?.EventSortOrder ?? 0;
            var boysEnvironment = boysPerformance?.Environment ?? Environment.None;
            var girlsAthleteName = girlsPerformance != null ? girlsPerformance.GirlsAthleteName : string.Empty;
            var girlsAthleteUrlName = girlsPerformance != null ? girlsPerformance.GirlsAthleteUrlName : string.Empty;
            var girlsPerformanceString = girlsPerformance != null ? girlsPerformance.GirlsPerformance : string.Empty;
            var girlsPersonalBest = girlsPerformance?.GirlsPersonalBest ?? false;
            var girlsEventSortOrder = girlsPerformance?.EventSortOrder ?? 0;
            var girlsEnvironment = girlsPerformance?.Environment ?? Environment.None;

            var both = new BestViewModel(
                boysAthleteName,
                boysAthleteUrlName,
                boysPerformanceString,
                boysPersonalBest,
                girlsAthleteName,
                girlsAthleteUrlName,
                girlsPerformanceString,
                girlsPersonalBest,
                eventName,
                boysEventSortOrder == 0 ? girlsEventSortOrder : boysEventSortOrder,
                boysEnvironment == Environment.None ? girlsEnvironment : boysEnvironment);
            
            combined.Add(both);
        }
        
        return combined.OrderBy(x => x.EventSortOrder).ThenBy(x => x.EventName).ToList();
    }

    private List<BestRelayViewModel> GetCombinedRelayBests(List<BestRelayViewModel> boysPerformances, List<BestRelayViewModel> girlsPerformances)
    {
        var combined = new List<BestRelayViewModel>();
        
        var eventNames = boysPerformances.Select(x => x.EventName).Union(girlsPerformances.Select(x => x.EventName));
        foreach (var eventName in eventNames)
        {
            var boysPerformance = boysPerformances.SingleOrDefault(x => x.EventName == eventName);
            var girlsPerformance = girlsPerformances.SingleOrDefault(x => x.EventName == eventName);

            var boysAthletesNames = boysPerformance != null ? boysPerformance.BoysAthletesNames : Enumerable.Empty<string>().ToList();
            var boysAthletesUrlNames = boysPerformance != null ? boysPerformance.BoysAthletesUrlNames : Enumerable.Empty<string>().ToList();
            var boysPerformanceString = boysPerformance != null ? boysPerformance.BoysPerformance : string.Empty;
            var boysPersonalBest = boysPerformance?.BoysPersonalBest ?? false;
            var boysEventSortOrder = boysPerformance?.EventSortOrder ?? 0;
            var boysEnvironment = boysPerformance?.Environment ?? Environment.None;
            var girlsAthletesNames = girlsPerformance != null ? girlsPerformance.GirlsAthletesNames : Enumerable.Empty<string>().ToList();
            var girlsAthletesUrlNames = girlsPerformance != null ? girlsPerformance.GirlsAthletesUrlNames : Enumerable.Empty<string>().ToList();
            var girlsPerformanceString = girlsPerformance != null ? girlsPerformance.GirlsPerformance : string.Empty;
            var girlsPersonalBest = girlsPerformance?.GirlsPersonalBest ?? false;
            var girlsEventSortOrder = girlsPerformance?.EventSortOrder ?? 0;
            var girlsEnvironment = girlsPerformance?.Environment ?? Environment.None;

            var both = new BestRelayViewModel(
                boysAthletesNames,
                boysAthletesUrlNames,
                boysPerformanceString,
                boysPersonalBest,
                girlsAthletesNames,
                girlsAthletesUrlNames,
                girlsPerformanceString,
                girlsPersonalBest,
                eventName,
                boysEventSortOrder == 0 ? girlsEventSortOrder : boysEventSortOrder,
                boysEnvironment == Environment.None ? girlsEnvironment : boysEnvironment);
            
            combined.Add(both);
        }
        
        return combined.OrderBy(x => x.EventSortOrder).ThenBy(x => x.EventName).ToList();
    }
}
