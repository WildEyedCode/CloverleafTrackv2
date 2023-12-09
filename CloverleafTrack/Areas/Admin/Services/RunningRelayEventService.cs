using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IRunningRelayEventService
{
    public List<RunningRelayEvent> Events { get; }
    public int Count { get; }
    public List<RunningRelayEvent> ReadAll();
    public RunningRelayEvent? ReadById(Guid id);
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(RunningRelayEvent @event, CancellationToken token);
    public Task UpdateAsync(RunningRelayEvent @event, CancellationToken token);
    public Task DeleteAsync(RunningRelayEvent @event, CancellationToken token);
}

public class RunningRelayEventService : IRunningRelayEventService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;
    
    public RunningRelayEventService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Events = new List<RunningRelayEvent>();
    }
    
    public List<RunningRelayEvent> Events { get; private set; }
    public int Count => Events.Count;

    public List<RunningRelayEvent> ReadAll()
    {
        return Events;
    }

    public RunningRelayEvent? ReadById(Guid id)
    {
        return Events.FirstOrDefault(x => x.Id == id);
    }

    public async Task ReloadAsync(CancellationToken token)
    {
        Events = (await connection.QueryAsync<RunningRelayEvent>(RunningRelayEventQueries.AllEventsSql)).ToList();
    }

    public async Task CreateAsync(RunningRelayEvent @event, CancellationToken token)
    {
        @event.Id = Guid.NewGuid();
        @event.DateCreated = DateTime.UtcNow;
        @event.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayEventQueries.CreateEventSql,
                @event,
                cancellationToken: token));
    }

    public async Task UpdateAsync(RunningRelayEvent @event, CancellationToken token)
    {
        @event.DateUpdated = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayEventQueries.UpdateEventSql,
                @event,
                cancellationToken: token));
    }

    public async Task DeleteAsync(RunningRelayEvent @event, CancellationToken token)
    {
        @event.Deleted = true;
        @event.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningRelayEventQueries.DeleteEventSql,
                @event,
                cancellationToken: token));
    }
}