using System.Data;
using CloverleafTrack.Areas.Admin.Queries;
using CloverleafTrack.Models;
using CloverleafTrack.Options;
using Dapper;
using Microsoft.Extensions.Options;

namespace CloverleafTrack.Areas.Admin.Services;

public interface IRunningPerformanceService
{
    public List<RunningPerformance> Performances { get; }
    public int Count { get; }
    public Task ReloadAsync(CancellationToken token);
    public Task CreateAsync(RunningPerformance performance, CancellationToken token);
    public List<RunningPerformance> ReadAll();
    public RunningPerformance? ReadById(Guid id);
    public Task UpdateAsync(RunningPerformance performance, CancellationToken token);
    public Task DeleteAsync(RunningPerformance performance, CancellationToken token);
}

public class RunningPerformanceService : IRunningPerformanceService
{
    private readonly IDbConnection connection;
    private readonly CloverleafTrackOptions options;

    public RunningPerformanceService(IDbConnection connection, IOptions<CloverleafTrackOptions> options)
    {
        this.connection = connection;
        this.options = options.Value;
        
        Performances = new List<RunningPerformance>();
    }
    
    public List<RunningPerformance> Performances { get; private set; }

    public int Count => Performances.Count;

    public async Task ReloadAsync(CancellationToken token)
    {
        Performances = (await connection.QueryAsync<RunningPerformance, RunningEvent, Meet, Season, Athlete, RunningPerformance>(RunningPerformanceQueries.AllPerformancesSql,
                (performance, @event, meet, season, athlete) =>
                {
                    performance.EventId = @event.Id;
                    performance.Event = @event;
                    performance.MeetId = meet.Id;
                    performance.Meet = meet;
                    performance.Meet.SeasonId = season.Id;
                    performance.Meet.Season = season;
                    performance.AthleteId = athlete.Id;
                    performance.Athlete = athlete;
                    return performance;
                },
                splitOn: "EventId,MeetId,SeasonId,AthleteId"))
            .OrderBy(x => x.Minutes).ThenBy(x => x.Seconds).ToList();
    }

    public async Task CreateAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.Id = Guid.NewGuid();
        performance.DateCreated = DateTime.UtcNow;
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningPerformanceQueries.CreatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public List<RunningPerformance> ReadAll()
    {
        return Performances;
    }

    public RunningPerformance? ReadById(Guid id)
    {
        return Performances.FirstOrDefault(x => x.Id == id);
    }

    public async Task UpdateAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.DateUpdated = DateTime.UtcNow;

        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningPerformanceQueries.UpdatePerformanceSql,
                performance,
                cancellationToken: token));

        await ReloadAsync(token);
    }

    public async Task DeleteAsync(RunningPerformance performance, CancellationToken token)
    {
        performance.Deleted = true;
        performance.DateDeleted = DateTime.UtcNow;
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                RunningPerformanceQueries.DeletePerformanceSql,
                performance,
                cancellationToken: token));
        
        await ReloadAsync(token);
    }
}