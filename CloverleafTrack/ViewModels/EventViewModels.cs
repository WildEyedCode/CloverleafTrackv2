using CloverleafTrack.Models;

namespace CloverleafTrack.ViewModels;

public record AllEventTypesViewModel(FieldEvent? FieldEvent, FieldRelayEvent? FieldRelayEvent, RunningEvent? RunningEvent, RunningRelayEvent? RunningRelayEvent);