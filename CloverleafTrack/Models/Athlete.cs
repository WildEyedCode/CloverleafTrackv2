using System.Web;

using Dapper.Contrib.Extensions;

namespace CloverleafTrack.Models;

public class Athlete : AuditModel
{
    [System.ComponentModel.DataAnnotations.Key] public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Gender { get; set; }
    public int GraduationYear { get; set; }
    [Computed] public string Name => $"{FirstName} {LastName}";
    [Computed] public string UrlName => $"{HttpUtility.UrlEncode(FirstName.ToLower())}-{HttpUtility.UrlEncode(LastName.ToLower())}";
    public override string ToString() => Name;
}
