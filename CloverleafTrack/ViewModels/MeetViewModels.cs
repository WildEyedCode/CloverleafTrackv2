using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record MeetsViewModel(
    List<SeasonMeetViewModel> Seasons);
public record SeasonMeetViewModel(
    Season Season,
    List<Meet> OutdoorMeets,
    List<Meet> IndoorMeets);
