using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IMeetService
{
    public List<Meet> Meets { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(Meet meet, CancellationToken token);
    public List<Meet> ReadAll();
    public Meet? ReadById(Guid id);
    public Task UpdateAsync(Meet meet, CancellationToken token);
    public Task DeleteAsync(Meet meet, CancellationToken token);
}

public class MeetService : IMeetService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public MeetService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;

        Meets = new List<Meet>();
    }
    
    public List<Meet> Meets { get; private set; }
    public int Count => Meets.Count;
    
    public async Task ReloadAsync(CancellationToken token)
    {
        Meets = (await connection.QueryAsync<Meet>(MeetQueries.AllMeetsSql)).ToList();
    }

    public async Task CreateAsync(Meet meet, CancellationToken token)
    {
        meet.Id = Guid.NewGuid();
        meet.DateCreated = DateTime.UtcNow;
        meet.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                MeetQueries.CreateMeetSql,
                meet,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<Meet> ReadAll()
    {
        return Meets;
    }

    public Meet? ReadById(Guid id)
    {
        return Meets.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(Meet meet, CancellationToken token)
    {
        meet.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                MeetQueries.UpdateMeetSql,
                meet,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(Meet meet, CancellationToken token)
    {
        meet.Deleted = true;
        meet.DateDeleted = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                MeetQueries.DeleteMeetSql,
                meet,
                cancellationToken: token));

        await ReloadAsync(token);
    }
}