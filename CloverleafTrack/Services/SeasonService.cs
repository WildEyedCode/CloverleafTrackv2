using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.Services;

public interface ISeasonService
{
    public List<Season> Seasons { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token = default);
    MeetsViewModel GetSeasonsWithMeets();
}

public class SeasonService : ISeasonService
{
    private readonly IDbConnection connection;

    public SeasonService(IDbConnection connection)
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
            splitOn: "MeetId"))
            .OrderByDescending(x => x.Name).ToList();

        Seasons = seasons.GroupBy(x => x.Id).Select(x =>
        {
            var groupedSeason = x.First();
            groupedSeason.Meets = x.Select(s => s.Meets.Single()).OrderByDescending(x => x.Date).ToList();
            return groupedSeason;
        }).ToList();
    }
    public MeetsViewModel GetSeasonsWithMeets()
    {
        return new MeetsViewModel(Seasons.Select(x => new SeasonMeetViewModel(x, x.Meets.FindAll(y => y.Environment == Environment.Outdoor), x.Meets.FindAll(y => y.Environment == Environment.Indoor))).ToList());
    }
}
