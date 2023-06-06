using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Queries;

using Dapper;

namespace CloverleafTrack.Managers;

public interface IAthleteManager
{
    public List<Athlete> Athletes { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
}

public class AthleteManager : IAthleteManager
{
    private readonly IDbConnection connection;

    public AthleteManager(IDbConnection connection)
    {
        this.connection = connection;
        Athletes = new List<Athlete>();
    }
    
    public List<Athlete> Athletes { get; private set; }
    public int Count => Athletes.Count;
    
    public async Task ReloadAsync(CancellationToken token)
    {
        Athletes = (await connection.QueryAsync<Athlete>(AthleteQueries.SelectAllAthletesSql)).ToList();
    }
}
