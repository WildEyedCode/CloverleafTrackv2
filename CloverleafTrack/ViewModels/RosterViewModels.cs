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
    List<RunningRelayEventPerformancesViewModel> OutdoorRunningRelayEventPerformances);

public record FieldEventPerformancesViewModel(FieldEvent Event, List<FieldPerformance> Performances);
public record FieldRelayEventPerformancesViewModel(FieldRelayEvent Event, List<FieldRelayPerformance> Performances);
public record RunningEventPerformancesViewModel(RunningEvent Event, List<RunningPerformance> Performances);
public record RunningRelayEventPerformancesViewModel(RunningRelayEvent Event, List<RunningRelayPerformance> Performances);