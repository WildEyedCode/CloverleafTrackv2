using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record MeetsViewModel(
    List<SeasonMeetViewModel> Seasons);
public record SeasonMeetViewModel(
    Season Season,
    List<Meet> OutdoorMeets,
    List<Meet> IndoorMeets);

public record MeetDetailsViewModel(
    Meet? Meet,
    List<MeetFieldEventViewModel> BoysFieldEvents,
    List<MeetFieldEventViewModel> GirlsFieldEvents,
    List<MeetFieldRelayEventViewModel> BoysFieldRelayEvents,
    List<MeetFieldRelayEventViewModel> GirlsFieldRelayEvents,
    List<MeetRunningEventViewModel> BoysRunningEvents,
    List<MeetRunningEventViewModel> GirlsRunningEvents,
    List<MeetRunningRelayEventViewModel> BoysRunningRelayEvents,
    List<MeetRunningRelayEventViewModel> GirlsRunningRelayEvents);
    
public record MeetFieldEventViewModel(
    FieldEvent Event,
    List<FieldPerformance> Performances);
    
public record MeetFieldRelayEventViewModel(
    FieldRelayEvent Event,
    List<FieldRelayPerformance> Performances);
    
public record MeetRunningEventViewModel(
    RunningEvent Event,
    List<RunningPerformance> Performances);
    
public record MeetRunningRelayEventViewModel(
    RunningRelayEvent Event,
    List<RunningRelayPerformance> Performances);