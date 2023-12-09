using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IRunningEventService
{
    public List<RunningEvent> Events { get; }
    public int Count { get; }
    public List<RunningEvent> ReadAll();
    public RunningEvent? ReadById(Guid id);
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(RunningEvent @event, CancellationToken token);
    public Task UpdateAsync(RunningEvent @event, CancellationToken token);
    public Task DeleteAsync(RunningEvent @event, CancellationToken token);
}

public class RunningEventService : IRunningEventService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;
    
    public RunningEventService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Events = new List<RunningEvent>();
    }
    
    public List<RunningEvent> Events { get; private set; }
    public int Count => Events.Count;

    public List<RunningEvent> ReadAll()
    {
        return Events;
    }

    public RunningEvent? ReadById(Guid id)
    {
        return Events.FirstOrDefault(x => x.Id == id);
    }

    public async Task ReloadAsync(CancellationToken token)
    {
        Events = (await connection.QueryAsync<RunningEvent>(RunningEventQueries.AllEventsSql)).ToList();
    }

    public async Task CreateAsync(RunningEvent @event, CancellationToken token)
    {
        @event.Id = Guid.NewGuid();
        @event.DateCreated = DateTime.UtcNow;
        @event.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningEventQueries.CreateEventSql,
                @event,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task UpdateAsync(RunningEvent @event, CancellationToken token)
    {
        @event.DateUpdated = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningEventQueries.UpdateEventSql,
                @event,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }

    public async Task DeleteAsync(RunningEvent @event, CancellationToken token)
    {
        @event.Deleted = true;
        @event.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningEventQueries.DeleteEventSql,
                @event,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
}