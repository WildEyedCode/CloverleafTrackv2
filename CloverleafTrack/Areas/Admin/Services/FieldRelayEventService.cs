using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IFieldRelayEventService
{
    public List<FieldRelayEvent> Events { get; }
    public int Count { get; }
    public List<FieldRelayEvent> ReadAll();
    public FieldRelayEvent? ReadById(Guid id);
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(FieldRelayEvent @event, CancellationToken token);
    public Task UpdateAsync(FieldRelayEvent @event, CancellationToken token);
    public Task DeleteAsync(FieldRelayEvent @event, CancellationToken token);
}

public class FieldRelayEventService : IFieldRelayEventService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;
    
    public FieldRelayEventService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Events = new List<FieldRelayEvent>();
    }
    
    public List<FieldRelayEvent> Events { get; private set; }
    public int Count => Events.Count;

    public List<FieldRelayEvent> ReadAll()
    {
        return Events;
    }

    public FieldRelayEvent? ReadById(Guid id)
    {
        return Events.FirstOrDefault(x => x.Id == id);
    }

    public async Task ReloadAsync(CancellationToken token)
    {
        Events = (await connection.QueryAsync<FieldRelayEvent>(FieldRelayEventQueries.AllEventsSql)).ToList();
    }

    public async Task CreateAsync(FieldRelayEvent @event, CancellationToken token)
    {
        @event.Id = Guid.NewGuid();
        @event.DateCreated = DateTime.UtcNow;
        @event.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayEventQueries.CreateEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task UpdateAsync(FieldRelayEvent @event, CancellationToken token)
    {
        @event.DateUpdated = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayEventQueries.UpdateEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(FieldRelayEvent @event, CancellationToken token)
    {
        @event.Deleted = true;
        @event.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldRelayEventQueries.DeleteEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }
}