using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

namespace CloverleafTrack.Managers;

public interface IEventManager
{
    public List<FieldEvent> FieldEvents { get; }
    public List<FieldRelayEvent> FieldRelayEvents { get; }
    public List<RunningEvent> RunningEvents { get; }
    public List<RunningRelayEvent> RunningRelayEvents { get; }
    public int FieldEventCount { get; }
    public int FieldRelayEventCount { get; }
    public int RunningEventCount { get; }
    public int RunningRelayEventCount { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
}

public class EventManager : IEventManager
{
    private readonly IDbConnection connection;

    public EventManager(IDbConnection connection)
    {
        this.connection = connection;
        FieldEvents = new List<FieldEvent>();
        FieldRelayEvents = new List<FieldRelayEvent>();
        RunningEvents = new List<RunningEvent>();
        RunningRelayEvents = new List<RunningRelayEvent>();
    }
    
    public List<FieldEvent> FieldEvents { get; private set; }
    public List<FieldRelayEvent> FieldRelayEvents { get; private set; }
    public List<RunningEvent> RunningEvents { get; private set; }
    public List<RunningRelayEvent> RunningRelayEvents { get; private set; }
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
    private async Task<List<FieldEvent>> ReloadFieldEventsAsync()
    {
        return (await connection.QueryAsync<FieldEvent>(EventQueries.SelectAllFieldEventsSql)).ToList();
    }
    private async Task<List<FieldRelayEvent>> ReloadFieldRelayEventsAsync()
    {
        return (await connection.QueryAsync<FieldRelayEvent>(EventQueries.SelectAllFieldRelayEventsSql)).ToList();
    }
    private async Task<List<RunningEvent>> ReloadRunningEventsAsync()
    {
        return (await connection.QueryAsync<RunningEvent>(EventQueries.SelectAllRunningEventsSql)).ToList();
    }
    private async Task<List<RunningRelayEvent>> ReloadRunningRelayEventsAsync()
    {
        return (await connection.QueryAsync<RunningRelayEvent>(EventQueries.SelectAllRunningRelayEventsSql)).ToList();
    }
}
