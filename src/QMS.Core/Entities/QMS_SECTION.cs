namespace QMS.Core.Entities;

public class QMS_SECTION : BaseEntity
{
    public string? NAME { get; set; }
    public string? REMARKS { get; set; }

    // Navigation Properties
    public virtual ICollection<QMS_SCREEN> Screens { get; set; } = new List<QMS_SCREEN>();
    public virtual ICollection<QMS_QUEUE> Queues { get; set; } = new List<QMS_QUEUE>();
}