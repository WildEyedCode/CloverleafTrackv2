using System.Web;

using Dapper.Contrib.Extensions;

namespace CloverleafTrack.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("Meets")]
public class Meet : AuditModel
{
    [System.ComponentModel.DataAnnotations.Key] public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool Outdoor { get; set; }
    public bool AllResultsIn { get; set; }
    public bool HandTimed { get; set; }
    public Environment? Environment { get; set; }
    public Guid SeasonId { get; set; }

    public Season Season { get; set; } = new();
    public List<FieldPerformance> FieldPerformances { get; set; } = new();
    
    [Computed] public string UrlName => $"{HttpUtility.UrlEncode(Name.Replace(" ", "-").ToLower())}";
    
    public override string ToString() => $"{Name} @ {Location} ({Date:d})";
}
