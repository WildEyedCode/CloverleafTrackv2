using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

namespace CloverleafTrack.Services;

public interface IMeetService
{
    public List<Meet> Meets { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
    public MeetDetailsViewModel? GetMeetDetailsByName(string meetName);
}

public class MeetService : IMeetService
{
    private readonly ISeasonService seasonService;
    private readonly IPerformanceService performanceService;
    private readonly IDbConnection connection;

    public MeetService(ISeasonService seasonService, IPerformanceService performanceService, IDbConnection connection)
    {
        this.seasonService = seasonService;
        this.performanceService = performanceService;
        this.connection = connection;
        
        Meets = new List<Meet>();
    }

    public List<Meet> Meets { get; private set; }
    public int Count => Meets.Count;
    
    public async Task ReloadAsync(CancellationToken token = default)
    {
        Meets = (await connection.QueryAsync<Meet, Season, Meet>(MeetQueries.AllMeetsSql,
            (meet, season) =>
            {
                meet.Season = season;
                return meet;
            },
            splitOn: "SeasonId")).ToList();
    }

    public MeetDetailsViewModel? GetMeetDetailsByName(string meetName)
    {
        var meet = Meets.FirstOrDefault(x => x.UrlName == meetName);
        if (meet == null)
        {
            return null;
        }

        var boysFieldEvents = GetFieldEvents(meet, Gender.Male);
        var girlsFieldEvents = GetFieldEvents(meet, Gender.Female);
        var boysFieldRelayEvents = GetFieldRelayEvents(meet, Gender.Male);
        var girlsFieldRelayEvents = GetFieldRelayEvents(meet, Gender.Female);
        var boysRunningEvents = GetRunningEvents(meet, Gender.Male);
        var girlsRunningEvents = GetRunningEvents(meet, Gender.Female);
        var boysRunningRelayEvents = GetRunningRelayEvents(meet, Gender.Male);
        var girlsRunningRelayEvents = GetRunningRelayEvents(meet, Gender.Female);

        return new MeetDetailsViewModel(
            meet,
            boysFieldEvents,
            girlsFieldEvents,
            boysFieldRelayEvents,
            girlsFieldRelayEvents,
            boysRunningEvents,
            girlsRunningEvents,
            boysRunningRelayEvents,
            girlsRunningRelayEvents);
    }

    private List<MeetFieldEventViewModel> GetFieldEvents(Meet meet, Gender gender)
    {
        return performanceService.FieldPerformances
            .FindAll(x => x.MeetId == meet.Id && x.Event.Gender == gender)
            .GroupBy(x => x.Event)
            .OrderBy(x => x.Key.SortOrder)
            .Select(x => new MeetFieldEventViewModel(x.Key, x.Select(p => p).ToList()))
            .ToList();
    }
    
    private List<MeetFieldRelayEventViewModel> GetFieldRelayEvents(Meet meet, Gender gender)
    {
        return performanceService.FieldRelayPerformances
            .FindAll(x => x.MeetId == meet.Id && x.Event.Gender == gender)
            .GroupBy(x => x.Event)
            .OrderBy(x => x.Key.SortOrder)
            .Select(x => new MeetFieldRelayEventViewModel(x.Key, x.Select(p => p).ToList()))
            .ToList();
    }
    
    private List<MeetRunningEventViewModel> GetRunningEvents(Meet meet, Gender gender)
    {
        return performanceService.RunningPerformances
            .FindAll(x => x.MeetId == meet.Id && x.Event.Gender == gender)
            .GroupBy(x => x.Event)
            .OrderBy(x => x.Key.SortOrder)
            .Select(x => new MeetRunningEventViewModel(x.Key, x.Select(p => p).ToList()))
            .ToList();
    }
    
    private List<MeetRunningRelayEventViewModel> GetRunningRelayEvents(Meet meet, Gender gender)
    {
        return performanceService.RunningRelayPerformances
            .FindAll(x => x.MeetId == meet.Id && x.Event.Gender == gender)
            .GroupBy(x => x.Event)
            .OrderBy(x => x.Key.SortOrder)
            .Select(x => new MeetRunningRelayEventViewModel(x.Key, x.Select(p => p).ToList()))
            .ToList();
    }
}
