using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

namespace CloverleafTrack.Managers;

public interface ISeasonManager
{
    public List<Season> Seasons { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
}

public class SeasonManager : ISeasonManager
{
    private readonly IDbConnection connection;

    public SeasonManager(IDbConnection connection)
    {
        this.connection = connection;
        Seasons = new List<Season>();
    }
    
    public List<Season> Seasons { get; private set; }
    public int Count => Seasons.Count;
    public async Task ReloadAsync(CancellationToken token)
    {
        var seasons = (await connection.QueryAsync<Season, Meet, Season>(SeasonQueries.SelectAllSeasonsSql,
            (season, meet) =>
            {
                season.Meets.Add(meet);
                return season;
            },
            splitOn: "MeetId")).ToList();

        Seasons = seasons.GroupBy(x => x.Id).Select(x =>
        {
            var groupedSeason = x.First();
            groupedSeason.Meets = x.Select(s => s.Meets.Single()).ToList();
            return groupedSeason;
        }).ToList();
    }
}
