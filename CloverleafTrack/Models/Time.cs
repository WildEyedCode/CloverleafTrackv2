namespace CloverleafTrack.Models;

public class Time : IEquatable<Time>, IComparable<Time>
{
    private const double SecondsPerMinute = 60;

    public double Minutes { get; set; }
    public double Seconds { get; set; }

    public double TotalMinutes => Minutes + (Seconds / SecondsPerMinute);
    public double TotalSeconds => (Minutes * SecondsPerMinute) + Seconds;

    public Time(double minutes, double seconds)
    {
        Minutes = minutes;
        Seconds = seconds;
    }
    
    private static Time FromSeconds(double seconds)
    {
        var minutes = Math.Truncate(seconds / SecondsPerMinute);
        var leftoverSeconds = seconds - (minutes * SecondsPerMinute);

        return new Time(minutes, leftoverSeconds);
    }

    public static Time operator +(Time a, Time b)
    {
        return FromSeconds(a.TotalSeconds + b.TotalSeconds);
    }

    public static Time operator -(Time a, Time b)
    {
        return FromSeconds(a.TotalSeconds - b.TotalSeconds);
    }

    public static Time operator -(Time a)
    {
        return FromSeconds(-a.TotalSeconds);
    }

    public static bool operator >(Time a, Time b)
    {
        return a.CompareTo(b) > 0;
    }
    
    public static bool operator <(Time a, Time b)
    {
        return a.CompareTo(b) < 0;
    }

    public override bool Equals(object? obj)
    {
        return obj is Time distance && Equals(distance);
    }

    public bool Equals(Time? other)
    {
        return other != null && Math.Abs(this.TotalSeconds - other.TotalSeconds) < 0.01;
    }

    public int CompareTo(Time? other)
    {
        return other != null ? this.TotalSeconds.CompareTo(other.TotalSeconds) : this.TotalSeconds.CompareTo(0);
    }

    public override int GetHashCode()
    {
        return this.TotalSeconds.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Minutes:00}:{Seconds:00.00}";
    }
}