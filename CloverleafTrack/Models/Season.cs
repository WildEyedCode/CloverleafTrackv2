using System.ComponentModel.DataAnnotations;

namespace CloverleafTrack.Models;

public class Season : AuditModel, IEquatable<Season>
{
    [Key] public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Meet> Meets { get; set; } = new();
    
    public bool Equals(Season? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Season) obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public override string ToString() => Name;
}
