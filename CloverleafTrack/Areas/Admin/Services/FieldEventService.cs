using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IFieldEventService
{
    public List<FieldEvent> Events { get; }
    public int Count { get; }
    public List<FieldEvent> ReadAll();
    public FieldEvent? ReadById(Guid id);
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(FieldEvent @event, CancellationToken token);
    public Task UpdateAsync(FieldEvent @event, CancellationToken token);
    public Task DeleteAsync(FieldEvent @event, CancellationToken token);
}

public class FieldEventService : IFieldEventService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;
    
    public FieldEventService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Events = new List<FieldEvent>();
    }
    
    public List<FieldEvent> Events { get; private set; }
    public int Count => Events.Count;

    public List<FieldEvent> ReadAll()
    {
        return Events;
    }

    public FieldEvent? ReadById(Guid id)
    {
        return Events.FirstOrDefault(x => x.Id == id);
    }

    public async Task ReloadAsync(CancellationToken token)
    {
        Events = (await connection.QueryAsync<FieldEvent>(FieldEventQueries.AllEventsSql)).ToList();
    }

    public async Task CreateAsync(FieldEvent @event, CancellationToken token)
    {
        @event.Id = Guid.NewGuid();
        @event.DateCreated = DateTime.UtcNow;
        @event.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldEventQueries.CreateEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task UpdateAsync(FieldEvent @event, CancellationToken token)
    {
        @event.DateUpdated = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldEventQueries.UpdateEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(FieldEvent @event, CancellationToken token)
    {
        @event.Deleted = true;
        @event.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                FieldEventQueries.DeleteEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }
}