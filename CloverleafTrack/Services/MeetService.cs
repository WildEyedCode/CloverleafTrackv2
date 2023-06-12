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
}

public class MeetService : IMeetService
{
    private readonly ISeasonService seasonService;
    private readonly IDbConnection connection;

    public MeetService(ISeasonService seasonService, IDbConnection connection)
    {
        this.seasonService = seasonService;
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
