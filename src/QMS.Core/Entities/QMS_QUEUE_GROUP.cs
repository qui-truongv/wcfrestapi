namespace QMS.Core.Entities;

public class QMS_QUEUE_GROUP : BaseEntity
{
    public string? NAME { get; set; }
    public string? REMARKS { get; set; }
    public string? CODE { get; set; }

    // Navigation Properties
    public virtual ICollection<QMS_QUEUE_TYPE> QueueTypes { get; set; } = new List<QMS_QUEUE_TYPE>();
}