using System.Data;

using CloverleafTrack.Models;
using CloverleafTrack.Options;
using CloverleafTrack.Queries;
using CloverleafTrack.ViewModels;

using Dapper;

using Microsoft.Extensions.Options;

namespace CloverleafTrack.Services;

public interface IAthleteService
{
    public List<Athlete> Athletes { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public List<Athlete> CurrentAthletes(bool gender);
    public List<Athlete> PastAthletes(bool gender);
    public RosterViewModel GetRoster();
}

public class AthleteService : IAthleteService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public AthleteService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        Athletes = new List<Athlete>();
    }
    
    public List<Athlete> Athletes { get; private set; }
    public int Count => Athletes.Count;
    
    public async Task ReloadAsync(CancellationToken token)
    {
        Athletes = (await connection.QueryAsync<Athlete>(AthleteQueries.SelectAllAthletesSql)).ToList();
    }

    public List<Athlete> CurrentAthletes(bool gender)
    {
        return Athletes
            .Where(x => x.GraduationYear >= options.GraduationYear && x.GraduationYear <= options.GraduationYear + 3 && x.Gender == gender)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public List<Athlete> PastAthletes(bool gender)
    {
        return Athletes
            .Where(x => x.GraduationYear < options.GraduationYear && x.Gender == gender)
            .OrderByDescending(x => x.GraduationYear)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public RosterViewModel GetRoster()
    {
        return new RosterViewModel(CurrentAthletes(false), CurrentAthletes(true), PastAthletes(false), PastAthletes(true));
    }
}
