using CloverleafTrack.Models;

using Environment = CloverleafTrack.Models.Environment;

namespace CloverleafTrack.ViewModels;

public record SeasonsViewModel(
    List<SeasonDetailViewModel> Seasons);

public record SeasonDetailViewModel(
    Season Season,
    List<BestViewModel> IndoorFieldPerformances,
    List<BestViewModel> OutdoorFieldPerformances,
    List<BestRelayViewModel> IndoorFieldRelayPerformances,
    List<BestRelayViewModel> OutdoorFieldRelayPerformances,
    List<BestViewModel> IndoorRunningPerformances,
    List<BestViewModel> OutdoorRunningPerformances,
    List<BestRelayViewModel> IndoorRunningRelayPerformances,
    List<BestRelayViewModel> OutdoorRunningRelayPerformances,
    List<Meet> IndoorMeets,
    List<Meet> OutdoorMeets
    );

public record BestViewModel(
    string BoysAthleteName,
    string BoysAthleteUrlName,
    string BoysPerformance,
    bool BoysSchoolRecord,
    bool BoysPersonalBest,
    string GirlsAthleteName,
    string GirlsAthleteUrlName,
    string GirlsPerformance,
    bool GirlsSchoolRecord,
    bool GirlsPersonalBest,
    string EventName,
    int EventSortOrder,
    Environment Environment);

public record BestRelayViewModel(
    List<string> BoysAthletesNames,
    List<string> BoysAthletesUrlNames,
    string BoysPerformance,
    bool BoysSchoolRecord,
    bool BoysPersonalBest,
    List<string> GirlsAthletesNames,
    List<string> GirlsAthletesUrlNames,
    string GirlsPerformance,
    bool GirlsSchoolRecord,
    bool GirlsPersonalBest,
    string EventName,
    int EventSortOrder,
    Environment Environment);