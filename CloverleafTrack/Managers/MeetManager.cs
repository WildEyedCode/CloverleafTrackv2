using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

namespace CloverleafTrack.Managers;

public interface IMeetManager
{
    public List<Meet> Meets { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
}

public class MeetManager : IMeetManager
{
    private readonly IDbConnection connection;

    public MeetManager(IDbConnection connection)
    {
        this.connection = connection;
        Meets = new List<Meet>();
    }

    public List<Meet> Meets { get; private set; }
    public int Count => Meets.Count;
    
    public async Task ReloadAsync(CancellationToken token = default)
    {
        Meets = (await connection.QueryAsync<Meet, Season, Meet>(MeetQueries.SelectAllMeetsSql,
            (meet, season) =>
            {
                meet.Season = season;
                return meet;
            },
            splitOn: "SeasonId")).ToList();
    }
}
