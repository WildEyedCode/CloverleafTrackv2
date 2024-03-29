using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface IEventService
{
    public List<FieldEvent> FieldEvents { get; }
    public List<FieldEvent> IndoorFieldEvents { get; }
    public List<FieldEvent> OutdoorFieldEvents { get; }
    public List<FieldRelayEvent> FieldRelayEvents { get; }
    public List<FieldRelayEvent> IndoorFieldRelayEvents { get; }
    public List<FieldRelayEvent> OutdoorFieldRelayEvents { get; }
    public List<RunningEvent> RunningEvents { get; }
    public List<RunningEvent> IndoorRunningEvents { get; }
    public List<RunningEvent> OutdoorRunningEvents { get; }
    public List<RunningRelayEvent> RunningRelayEvents { get; }
    public List<RunningRelayEvent> IndoorRunningRelayEvents { get; }
    public List<RunningRelayEvent> OutdoorRunningRelayEvents { get; }
    public int FieldEventCount { get; }
    public int FieldRelayEventCount { get; }
    public int RunningEventCount { get; }
    public int RunningRelayEventCount { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public AllEventTypesViewModel GetEventByName(string eventName);
}

public class EventService : IEventService
{
    private readonly IDbConnection connection;

    public EventService(IDbConnection connection)
    {
        this.connection = connection;
        FieldEvents = new List<FieldEvent>();
        FieldRelayEvents = new List<FieldRelayEvent>();
        RunningEvents = new List<RunningEvent>();
        RunningRelayEvents = new List<RunningRelayEvent>();
    }
    
    public List<FieldEvent> FieldEvents { get; private set; }
    public List<FieldEvent> IndoorFieldEvents => FieldEvents.FindAll(x => x.Environment == Environment.Indoor);
    public List<FieldEvent> OutdoorFieldEvents => FieldEvents.FindAll(x => x.Environment == Environment.Outdoor);
    public List<FieldRelayEvent> FieldRelayEvents { get; private set; }
    public List<FieldRelayEvent> IndoorFieldRelayEvents => FieldRelayEvents.FindAll(x => x.Environment == Environment.Indoor);
    public List<FieldRelayEvent> OutdoorFieldRelayEvents => FieldRelayEvents.FindAll(x => x.Environment == Environment.Outdoor);
    public List<RunningEvent> RunningEvents { get; private set; }
    public List<RunningEvent> IndoorRunningEvents => RunningEvents.FindAll(x => x.Environment == Environment.Indoor);
    public List<RunningEvent> OutdoorRunningEvents => RunningEvents.FindAll(x => x.Environment == Environment.Outdoor);
    public List<RunningRelayEvent> RunningRelayEvents { get; private set; }
    public List<RunningRelayEvent> IndoorRunningRelayEvents => RunningRelayEvents.FindAll(x => x.Environment == Environment.Indoor);
    public List<RunningRelayEvent> OutdoorRunningRelayEvents => RunningRelayEvents.FindAll(x => x.Environment == Environment.Outdoor);
    public int FieldEventCount => FieldEvents.Count;
    public int FieldRelayEventCount => FieldRelayEvents.Count;
    public int RunningEventCount => RunningEvents.Count;
    public int RunningRelayEventCount => RunningRelayEvents.Count;
    public int Count => FieldEventCount + FieldRelayEventCount + RunningEventCount + RunningRelayEventCount;

    public async Task ReloadAsync(CancellationToken token)
    {
        FieldEvents = await ReloadFieldEventsAsync();
        FieldRelayEvents = await ReloadFieldRelayEventsAsync();
        RunningEvents = await ReloadRunningEventsAsync();
        RunningRelayEvents = await ReloadRunningRelayEventsAsync();
    }

    public AllEventTypesViewModel GetEventByName(string eventName)
    {
        var fieldEvent = FieldEvents.SingleOrDefault(x => x.UrlName == eventName);
        var fieldRelayEvent = FieldRelayEvents.SingleOrDefault(x => x.UrlName == eventName);
        var runningEvent = RunningEvents.SingleOrDefault(x => x.UrlName == eventName);
        var runningRelayEvent = RunningRelayEvents.SingleOrDefault(x => x.UrlName == eventName);
        
        return new AllEventTypesViewModel(fieldEvent, fieldRelayEvent, runningEvent, runningRelayEvent);
    }
    
    private async Task<List<FieldEvent>> ReloadFieldEventsAsync()
    {
        return (await connection.QueryAsync<FieldEvent>(EventQueries.AllFieldEventsSql))
            .OrderBy(x => x.SortOrder).ToList();
    }
    private async Task<List<FieldRelayEvent>> ReloadFieldRelayEventsAsync()
    {
        return (await connection.QueryAsync<FieldRelayEvent>(EventQueries.AllFieldRelayEventsSql))
            .OrderBy(x => x.SortOrder).ToList();
    }
    private async Task<List<RunningEvent>> ReloadRunningEventsAsync()
    {
        return (await connection.QueryAsync<RunningEvent>(EventQueries.AllRunningEventsSql))
            .OrderBy(x => x.SortOrder).ToList();
    }
    private async Task<List<RunningRelayEvent>> ReloadRunningRelayEventsAsync()
    {
        return (await connection.QueryAsync<RunningRelayEvent>(EventQueries.AllRunningRelayEventsSql))
            .OrderBy(x => x.SortOrder).ToList();
    }
}
