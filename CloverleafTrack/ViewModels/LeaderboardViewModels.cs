using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record LeaderboardViewModel(
    GenderEventsViewModel BoysEvents,
    GenderEventsViewModel GirlsEvents,
    GenderLeaderboardViewModel BoysLeaderboard,
    GenderLeaderboardViewModel GirlsLeaderboard);

public record GenderEventsViewModel(
    List<FieldEvent> IndoorFieldEvents,
    List<FieldEvent> OutdoorFieldEvents,
    List<FieldRelayEvent> IndoorFieldRelayEvents,
    List<FieldRelayEvent> OutdoorFieldRelayEvents,
    List<RunningEvent> IndoorRunningEvents,
    List<RunningEvent> OutdoorRunningEvents,
    List<RunningRelayEvent> IndoorRunningRelayEvents,
    List<RunningRelayEvent> OutdoorRunningRelayEvents);

public record GenderLeaderboardViewModel(
    List<FieldLeaderboardViewModel> IndoorFieldLeaderboards,
    List<FieldLeaderboardViewModel> OutdoorFieldLeaderboards,
    List<FieldRelayLeaderboardViewModel> IndoorFieldRelayLeaderboards,
    List<FieldRelayLeaderboardViewModel> OutdoorFieldRelayLeaderboards,
    List<RunningLeaderboardViewModel> IndoorRunningLeaderboards,
    List<RunningLeaderboardViewModel> OutdoorRunningLeaderboards,
    List<RunningRelayLeaderboardViewModel> IndoorRunningRelayLeaderboards,
    List<RunningRelayLeaderboardViewModel> OutdoorRunningRelayLeaderboards);

public record FieldLeaderboardViewModel(
    FieldEvent Event,
    FieldPerformance Performance,
    Athlete Athlete);

public record FieldRelayLeaderboardViewModel(
    FieldRelayEvent Event,
    FieldRelayPerformance Performance,
    List<Athlete> Athletes);

public record RunningLeaderboardViewModel(
    RunningEvent Event,
    RunningPerformance Performance,
    Athlete Athlete);

public record RunningRelayLeaderboardViewModel(
    RunningRelayEvent Event,
    RunningRelayPerformance Performance,
    List<Athlete> Athletes);

public record FieldEventLeaderboardViewModel(
    FieldEvent Event,
    List<FieldLeaderboardViewModel> Leaderboards,
    bool PersonalRecordsOnly);

public record RunningEventLeaderboardViewModel(
    RunningEvent Event,
    List<RunningLeaderboardViewModel> Leaderboards,
    bool PersonalRecordsOnly);

public record FieldRelayEventLeaderboardViewModel(
    FieldRelayEvent Event,
    List<FieldRelayLeaderboardViewModel> Leaderboards,
    bool PersonalRecordsOnly);

public record RunningRelayEventLeaderboardViewModel(
    RunningRelayEvent Event,
    List<RunningRelayLeaderboardViewModel> Leaderboards,
    bool PersonalRecordsOnly);

