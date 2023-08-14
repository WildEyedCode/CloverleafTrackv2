using System.ComponentModel.DataAnnotations;

namespace CloverleafTrack.Models;

public abstract class AuditModel
{
    public bool Deleted { get; set; }
    
    [Display(Name = "Date Created")]
    public DateTime DateCreated { get; set; }
    
    [Display(Name = "Date Updated")]
    public DateTime? DateUpdated { get; set; }
    
    [Display(Name = "Date Deleted")]
    public DateTime? DateDeleted { get; set; }
}
