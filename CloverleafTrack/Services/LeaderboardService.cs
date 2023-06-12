using CloverleafTrack.Models;
using CloverleafTrack.ViewModels;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface ILeaderboardService
{
    public LeaderboardViewModel GetFullLeaderboard();
    public FieldEventLeaderboardViewModel GetFieldEventLeaderboard(FieldEvent @event, bool personalRecordsOnly = false);
    public FieldRelayEventLeaderboardViewModel GetFieldRelayEventLeaderboard(FieldRelayEvent @event, bool personalRecordsOnly = false);
    public RunningEventLeaderboardViewModel GetRunningEventLeaderboard(RunningEvent @event, bool personalRecordsOnly = false);
    public RunningRelayEventLeaderboardViewModel GetRunningRelayEventLeaderboard(RunningRelayEvent @event, bool personalRecordsOnly = false);
}

public class LeaderboardService : ILeaderboardService
{
    private readonly IAthleteService athleteService;
    private readonly IEventService eventService;
    private readonly IPerformanceService performanceService;

    public LeaderboardService(IAthleteService athleteService, IEventService eventService, IPerformanceService performanceService)
    {
        this.athleteService = athleteService;
        this.eventService = eventService;
        this.performanceService = performanceService;
    }

    public LeaderboardViewModel GetFullLeaderboard()
    {
        var boysEvents = GetGenderEvents(Gender.Male);
        var girlsEvents = GetGenderEvents(Gender.Female);
        var boysLeaderboard = GetGenderLeaderboard(Gender.Male);
        var girlsLeaderboard = GetGenderLeaderboard(Gender.Female);

        return new LeaderboardViewModel(boysEvents, girlsEvents, boysLeaderboard, girlsLeaderboard);
    }
    public FieldEventLeaderboardViewModel GetFieldEventLeaderboard(FieldEvent @event, bool personalRecordsOnly = false)
    {
        var performances = performanceService.FieldPerformances.FindAll(x => x.EventId == @event.Id);
        var ordered = performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches);
        var leaderboards = new List<FieldLeaderboardViewModel>();

        foreach (var performance in ordered)
        {
            switch (personalRecordsOnly)
            {
                case true when leaderboards.All(x => x.Athlete.Id != performance.Athlete.Id):
                {
                    var leaderboard = new FieldLeaderboardViewModel(@event, performance, performance.Athlete);
                    leaderboards.Add(leaderboard);
                    break;
                }
                case false:
                {
                    var leaderboard = new FieldLeaderboardViewModel(@event, performance, performance.Athlete);
                    leaderboards.Add(leaderboard);
                    break;
                }
            }

        }
        
        return new FieldEventLeaderboardViewModel(@event, leaderboards, personalRecordsOnly);
    }
    public FieldRelayEventLeaderboardViewModel GetFieldRelayEventLeaderboard(FieldRelayEvent @event, bool personalRecordsOnly = false)
    {
        var performances = performanceService.FieldRelayPerformances.FindAll(x => x.EventId == @event.Id);
        var ordered = performances.OrderByDescending(x => x.Feet).ThenByDescending(x => x.Inches);
        var leaderboards = new List<FieldRelayLeaderboardViewModel>();

        foreach (var performance in ordered)
        {
            var athletes = new List<Athlete>();
            var athleteIdHashSet = new HashSet<Guid>();
            foreach (var athleteId in performance.Athletes.Select(x => x.Id))
            {
                var athlete = athleteService.Athletes.SingleOrDefault(x => x.Id == athleteId);
                if (athlete != null)
                {
                    athletes.Add(athlete);
                    athleteIdHashSet.Add(athlete.Id);
                }
            }

            
            if (personalRecordsOnly)
            {
                var allLeaderboardsAthleteIds = new List<HashSet<Guid>>();
                foreach (var item in leaderboards)
                {
                    var athleteIds = new HashSet<Guid>();
                    foreach (var athlete in item.Athletes)
                    {
                        athleteIds.Add(athlete.Id);
                    }

                    allLeaderboardsAthleteIds.Add(athleteIds);
                }

                if (allLeaderboardsAthleteIds.Any(x => x.SetEquals(athleteIdHashSet))) continue;

                var leaderboard = new FieldRelayLeaderboardViewModel(@event, performance, athletes);
                leaderboards.Add(leaderboard);
            }
            else
            {
                var leaderboard = new FieldRelayLeaderboardViewModel(@event, performance, athletes);
                leaderboards.Add(leaderboard);
            }
        }
        
        return new FieldRelayEventLeaderboardViewModel(@event, leaderboards, personalRecordsOnly);
    }
    public RunningEventLeaderboardViewModel GetRunningEventLeaderboard(RunningEvent @event, bool personalRecordsOnly = false)
    {
        var performances = performanceService.RunningPerformances.FindAll(x => x.EventId == @event.Id);
        var ordered = performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds);
        var leaderboards = new List<RunningLeaderboardViewModel>();

        foreach (var performance in ordered)
        {
            switch (personalRecordsOnly)
            {
                case true when leaderboards.All(x => x.Athlete.Id != performance.Athlete.Id):
                {
                    var leaderboard = new RunningLeaderboardViewModel(@event, performance, performance.Athlete);
                    leaderboards.Add(leaderboard);
                    break;
                }
                case false:
                {
                    var leaderboard = new RunningLeaderboardViewModel(@event, performance, performance.Athlete);
                    leaderboards.Add(leaderboard);
                    break;
                }
            }

        }
        
        return new RunningEventLeaderboardViewModel(@event, leaderboards, personalRecordsOnly);
    }
    public RunningRelayEventLeaderboardViewModel GetRunningRelayEventLeaderboard(RunningRelayEvent @event, bool personalRecordsOnly = false)
    {
        var performances = performanceService.RunningRelayPerformances.FindAll(x => x.EventId == @event.Id);
        var ordered = performances.OrderBy(x => x.Minutes).ThenBy(x => x.Seconds);
        var leaderboards = new List<RunningRelayLeaderboardViewModel>();

        foreach (var performance in ordered)
        {
            var athletes = new List<Athlete>();
            var athleteIdHashSet = new HashSet<Guid>();
            foreach (var athleteId in performance.Athletes.Select(x => x.Id))
            {
                var athlete = athleteService.Athletes.SingleOrDefault(x => x.Id == athleteId);
                if (athlete != null)
                {
                    athletes.Add(athlete);
                    athleteIdHashSet.Add(athlete.Id);
                }
            }

            
            if (personalRecordsOnly)
            {
                var allLeaderboardsAthleteIds = new List<HashSet<Guid>>();
                foreach (var item in leaderboards)
                {
                    var athleteIds = new HashSet<Guid>();
                    foreach (var athlete in item.Athletes)
                    {
                        athleteIds.Add(athlete.Id);
                    }

                    allLeaderboardsAthleteIds.Add(athleteIds);
                }

                if (allLeaderboardsAthleteIds.Any(x => x.SetEquals(athleteIdHashSet))) continue;

                var leaderboard = new RunningRelayLeaderboardViewModel(@event, performance, athletes);
                leaderboards.Add(leaderboard);
            }
            else
            {
                var leaderboard = new RunningRelayLeaderboardViewModel(@event, performance, athletes);
                leaderboards.Add(leaderboard);
            }
        }
        
        return new RunningRelayEventLeaderboardViewModel(@event, leaderboards, personalRecordsOnly);
    }
    private GenderLeaderboardViewModel GetGenderLeaderboard(Gender gender)
    {
        return new GenderLeaderboardViewModel(
            performanceService.FieldPerformances.Where(x => x.Event.Environment == Environment.Indoor && x.Event.Gender == gender).Select(x => new FieldLeaderboardViewModel(x.Event, x, x.Athlete)).ToList(),
            performanceService.FieldPerformances.Where(x => x.Event.Environment == Environment.Outdoor && x.Event.Gender == gender).Select(x => new FieldLeaderboardViewModel(x.Event, x, x.Athlete)).ToList(),
            performanceService.FieldRelayPerformances.Where(x => x.Event.Environment == Environment.Indoor && x.Event.Gender == gender).Select(x => new FieldRelayLeaderboardViewModel(x.Event, x, x.Athletes)).ToList(),
            performanceService.FieldRelayPerformances.Where(x => x.Event.Environment == Environment.Outdoor && x.Event.Gender == gender).Select(x => new FieldRelayLeaderboardViewModel(x.Event, x, x.Athletes)).ToList(),
            performanceService.RunningPerformances.Where(x => x.Event.Environment == Environment.Indoor && x.Event.Gender == gender).Select(x => new RunningLeaderboardViewModel(x.Event, x, x.Athlete)).ToList(),
            performanceService.RunningPerformances.Where(x => x.Event.Environment == Environment.Outdoor && x.Event.Gender == gender).Select(x => new RunningLeaderboardViewModel(x.Event, x, x.Athlete)).ToList(),
            performanceService.RunningRelayPerformances.Where(x => x.Event.Environment == Environment.Indoor && x.Event.Gender == gender).Select(x => new RunningRelayLeaderboardViewModel(x.Event, x, x.Athletes)).ToList(),
            performanceService.RunningRelayPerformances.Where(x => x.Event.Environment == Environment.Outdoor && x.Event.Gender == gender).Select(x => new RunningRelayLeaderboardViewModel(x.Event, x, x.Athletes)).ToList());
    }

    private GenderEventsViewModel GetGenderEvents(Gender gender)
    {
        return new GenderEventsViewModel(
            eventService.FieldEvents.FindAll(x => x.Environment == Environment.Indoor && x.Gender == gender),
            eventService.FieldEvents.FindAll(x => x.Environment == Environment.Outdoor && x.Gender == gender),
            eventService.FieldRelayEvents.FindAll(x => x.Environment == Environment.Indoor && x.Gender == gender),
            eventService.FieldRelayEvents.FindAll(x => x.Environment == Environment.Outdoor && x.Gender == gender),
            eventService.RunningEvents.FindAll(x => x.Environment == Environment.Indoor && x.Gender == gender),
            eventService.RunningEvents.FindAll(x => x.Environment == Environment.Outdoor && x.Gender == gender),
            eventService.RunningRelayEvents.FindAll(x => x.Environment == Environment.Indoor && x.Gender == gender),
            eventService.RunningRelayEvents.FindAll(x => x.Environment == Environment.Outdoor && x.Gender == gender));
    }
}
