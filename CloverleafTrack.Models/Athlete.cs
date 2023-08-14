using System.ComponentModel.DataAnnotations;
using System.Web;

using Dapper.Contrib.Extensions;

namespace CloverleafTrack.Models;

public class Athlete : AuditModel
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
}
