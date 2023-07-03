using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record RosterViewModel(
    List<Athlete> CurrentBoysAthletes,
    List<Athlete> CurrentGirlsAthletes,
    List<Athlete> PastBoysAthletes,
    List<Athlete> PastGirlsAthletes);

public record AthleteViewModel(
    Athlete? Athlete,
    List<FieldEventPerformancesViewModel> IndoorFieldEventPerformances,
    List<FieldEventPerformancesViewModel> OutdoorFieldEventPerformances,
    List<FieldRelayEventPerformancesViewModel> IndoorFieldRelayEventPerformances,
    List<FieldRelayEventPerformancesViewModel> OutdoorFieldRelayEventPerformances,
    List<RunningEventPerformancesViewModel> IndoorRunningEventPerformances,
    List<RunningEventPerformancesViewModel> OutdoorRunningEventPerformances,
    List<RunningRelayEventPerformancesViewModel> IndoorRunningRelayEventPerformances,
    List<RunningRelayEventPerformancesViewModel> OutdoorRunningRelayEventPerformances,
    List<Season> Seasons,
    List<FieldEventSeasonViewModel> IndoorFieldSeasonBests,
    List<FieldEventSeasonViewModel> OutdoorFieldSeasonBests,
    List<FieldRelayEventSeasonViewModel> IndoorFieldRelaySeasonBests,
    List<FieldRelayEventSeasonViewModel> OutdoorFieldRelaySeasonBests,
    List<RunningEventSeasonViewModel> IndoorRunningSeasonBests,
    List<RunningEventSeasonViewModel> OutdoorRunningSeasonBests,
    List<RunningRelayEventSeasonViewModel> IndoorRunningRelaySeasonBests,
    List<RunningRelayEventSeasonViewModel> OutdoorRunningRelaySeasonBests,
    List<FieldEventBestViewModel> IndoorFieldEventLifetimeBests,
    List<FieldEventBestViewModel> OutdoorFieldEventLifetimeBests,
    List<FieldRelayEventBestViewModel> IndoorFieldRelayEventLifetimeBests,
    List<FieldRelayEventBestViewModel> OutdoorFieldRelayEventLifetimeBests,
    List<RunningEventBestViewModel> IndoorRunningEventLifetimeBests,
    List<RunningEventBestViewModel> OutdoorRunningEventLifetimeBests,
    List<RunningRelayEventBestViewModel> IndoorRunningRelayEventLifetimeBests,
    List<RunningRelayEventBestViewModel> OutdoorRunningRelayEventLifetimeBests,
    bool HasIndoorPerformances,
    bool HasOutdoorPerformances,
    bool HasIndoorLifetimeBests,
    bool HasOutdoorLifetimeBests,
    bool HasLifetimeBests,
    bool HasIndoorSeasonBests,
    bool HasOutdoorSeasonBests,
    bool HasSeasonBests);

public record FieldEventPerformancesViewModel(FieldEvent Event, List<FieldPerformance> Performances);
public record FieldRelayEventPerformancesViewModel(FieldRelayEvent Event, List<FieldRelayPerformance> Performances);
public record RunningEventPerformancesViewModel(RunningEvent Event, List<RunningPerformance> Performances);
public record RunningRelayEventPerformancesViewModel(RunningRelayEvent Event, List<RunningRelayPerformance> Performances);

public record FieldEventSeasonViewModel(Season Season, List<FieldEventBestViewModel> Performances);
public record FieldRelayEventSeasonViewModel(Season Season, List<FieldRelayEventBestViewModel> Performances);
public record RunningEventSeasonViewModel(Season Season, List<RunningEventBestViewModel> Performances);
public record RunningRelayEventSeasonViewModel(Season Season, List<RunningRelayEventBestViewModel> Performances);
public record FieldEventBestViewModel(FieldEvent Event, FieldPerformance Performance);
public record FieldRelayEventBestViewModel(FieldRelayEvent Event, FieldRelayPerformance Performance);
public record RunningEventBestViewModel(RunningEvent Event, RunningPerformance Performance);
public record RunningRelayEventBestViewModel(RunningRelayEvent Event, RunningRelayPerformance Performance);