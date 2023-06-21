using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record RosterViewModel(List<Athlete> CurrentBoysAthletes, List<Athlete> CurrentGirlsAthletes, List<Athlete> PastBoysAthletes, List<Athlete> PastGirlsAthletes);
