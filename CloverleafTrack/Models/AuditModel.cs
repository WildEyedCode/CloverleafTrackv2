namespace CloverleafTrack.Models;

public abstract class AuditModel
{
    public bool Deleted { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public DateTime? DateDeleted { get; set; }
}
