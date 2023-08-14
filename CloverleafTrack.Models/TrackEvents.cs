using System.Text;
using System.Web;

using Dapper.Contrib.Extensions;

namespace CloverleafTrack.Models;

public abstract class TrackEvent<T> : AuditModel, IEquatable<TrackEvent<T>> where T : new()
{
    [System.ComponentModel.DataAnnotations.Key] public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public Environment Environment { get; set; }
    public int SortOrder { get; set; }
    public List<T> Performances { get; set; } = new();
    
    [Computed]
    public string DisplayName
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append(Gender == Gender.Female ? "Girls " : "Boys ");
            builder.Append(Environment == Environment.Outdoor ? "Outdoor " : "Indoor ");
            builder.Append(Name);

            return builder.ToString();
        }
    }

    [Computed]
    public string UrlName
    {
        get
        {
            var builder = new StringBuilder();

            builder.Append(Gender == Gender.Female ? "girls-" : "boys-");
            builder.Append(Environment == Environment.Outdoor ? "outdoor-" : "indoor-");
            builder.Append(HttpUtility.UrlDecode(Name.Replace(" ", "-").ToLower()));
            
            return builder.ToString();
        }
    }

    public override string ToString() => DisplayName;
    
    public bool Equals(TrackEvent<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((TrackEvent<T>) obj);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public class FieldEvent : TrackEvent<FieldPerformance>
{
}

public class FieldRelayEvent : TrackEvent<FieldRelayPerformance>
{
}

public class RunningEvent : TrackEvent<RunningPerformance>
{
}

public class RunningRelayEvent : TrackEvent<RunningRelayPerformance>
{
}