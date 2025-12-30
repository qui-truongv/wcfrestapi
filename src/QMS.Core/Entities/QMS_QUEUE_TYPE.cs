namespace QMS.Core.Entities;

public class QMS_QUEUE_TYPE : BaseEntity
{
    public string? NAME { get; set; }
    public string? REMARKS { get; set; }
    public int QUEUEGROUP_ID { get; set; }
    public string? QUEUE_TYPE_CODE { get; set; }

    // Navigation Properties
    public virtual QMS_QUEUE_GROUP QueueGroup { get; set; } = null!;
    public virtual ICollection<QMS_QUEUE> Queues { get; set; } = new List<QMS_QUEUE>();
}