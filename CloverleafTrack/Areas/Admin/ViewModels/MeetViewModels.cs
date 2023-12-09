using CloverleafTrack.Models;

namespace CloverleafTrack.Areas.Admin.ViewModels;

public record MeetAndSeasonsViewModel(Meet Meet, List<Season> Seasons);