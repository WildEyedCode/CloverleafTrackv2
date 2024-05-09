using System.ComponentModel.DataAnnotations;
using System.Web;

using Dapper.Contrib.Extensions;

namespace CloverleafTrack.Models;

public class Athlete : AuditModel, IEquatable<Athlete>
{
    [System.ComponentModel.DataAnnotations.Key]
    public Guid Id { get; set; }
    
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;
    
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;
    
    public bool Gender { get; set; }
    
    [Display(Name = "Graduation Year")]
    public int GraduationYear { get; set; }
    
    [Computed]
    public string Name => $"{FirstName} {LastName}";
    
    [Computed]
    [Display(Name = "URL-Safe Name")]
    public string UrlName => $"{HttpUtility.UrlEncode(FirstName.ToLower())}-{HttpUtility.UrlEncode(LastName.ToLower())}";
    
    public override string ToString() => Name;

    public bool Equals(Athlete? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Athlete)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
