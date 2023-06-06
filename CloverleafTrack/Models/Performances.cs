using System.ComponentModel.DataAnnotations;

namespace CloverleafTrack.Models;

public abstract class Performance<T> : AuditModel where T : new()
{
    [Key] public Guid Id { get; set; }
    public bool SchoolRecord { get; set; }
    public bool SeasonBest { get; set; }
    public bool PersonalBest { get; set; }
    public Guid EventId { get; set; }
    public Guid MeetId { get; set; }
    public T Event { get; set; } = new();
    public Meet Meet { get; set; } = new();
}

public abstract class IndividualPerformance<T> : Performance<T> where T : new()
{
    public Guid AthleteId { get; set; }
    public Athlete Athlete { get; set; } = new();
}

public abstract class RelayPerformance<T> : Performance<T> where T : new()
{
    public List<Guid> AthleteIds { get; set; } = new();
    public List<Athlete> Athletes { get; set; } = new();
}

public class FieldPerformance : IndividualPerformance<FieldEvent>
{
    public double Feet { get; set; }
    public double Inches { get; set; }
    public Distance Distance => new Distance(Feet, Inches);
    public override string ToString() => $"{Distance} in {Event} by {Athlete} at {Meet}";
}

public class FieldRelayPerformance : RelayPerformance<FieldRelayEvent>
{
    public double Feet { get; set; }
    public double Inches { get; set; }
    public Distance Distance => new Distance(Feet, Inches);
    public override string ToString() => $"{Distance} in {Event} by {Athletes} at {Meet}";
}

public class RunningPerformance : IndividualPerformance<RunningEvent>
{
    public double Minutes { get; set; }
    public double Seconds { get; set; }
    public Time Time => new Time(Minutes, Seconds);
    public override string ToString() => $"{Time} in {Event} by {Athlete} at {Meet}";
}

public class RunningRelayPerformance : RelayPerformance<RunningRelayEvent>
{
    public double Minutes { get; set; }
    public double Seconds { get; set; }
    public Time Time => new Time(Minutes, Seconds);
    public override string ToString() => $"{Time} in {Event} by {Athletes} at {Meet}";
}