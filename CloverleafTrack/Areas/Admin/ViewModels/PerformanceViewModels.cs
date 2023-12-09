using CloverleafTrack.Models;

namespace CloverleafTrack.Areas.Admin.ViewModels;

public record FullFieldPerformanceViewModel(FieldPerformance Performance, List<FieldEvent> Events, List<Meet> Meets, List<Athlete> Athletes);
public record FullFieldRelayPerformanceViewModel(FieldRelayPerformance Performance, Guid AthleteId1, Guid AthleteId2, Guid AthleteId3, List<FieldRelayEvent> Events, List<Meet> Meets, List<Athlete> Athletes);
public record FullRunningPerformanceViewModel(RunningPerformance Performance, List<RunningEvent> Events, List<Meet> Meets, List<Athlete> Athletes);
public record FullRunningRelayPerformanceViewModel(RunningRelayPerformance Performance, Guid AthleteId1, Guid AthleteId2, Guid AthleteId3, Guid AthleteId4, List<RunningRelayEvent> Events, List<Meet> Meets, List<Athlete> Athletes);