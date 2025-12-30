namespace QMS.Core.Entities;

public class QMS_QUEUE : BaseEntity
{
    public string NAME { get; set; } = string.Empty;
    public int? STATE { get; set; }
    public int? SCREEN_ID { get; set; }
    public int? DEPARTMENT_ID { get; set; }
    public int? MAX { get; set; }
    public int? QUEUETYPE_ID { get; set; }
    public int? SECTION_ID { get; set; }
    public string? REMARKS { get; set; }
    public string? TENBACSI { get; set; }
    public string? TENDIEUDUONG { get; set; }
    public int IS_MANUAL { get; set; } = 0;
    public int? IDX { get; set; }
    public string? CODESCREEN { get; set; }

    // Navigation Properties
    public virtual QMS_SCREEN? Screen { get; set; }
    public virtual QMS_SECTION? Section { get; set; }
    public virtual QMS_QUEUE_TYPE? QueueType { get; set; }
    public virtual ICollection<QMS_KIOSK_QUEUE> KioskQueues { get; set; } = new List<QMS_KIOSK_QUEUE>();
    public virtual ICollection<QMS_QUEUE_ITEM> QueueItems { get; set; } = new List<QMS_QUEUE_ITEM>();
    public virtual ICollection<QMS_CLIENT> Clients { get; set; } = new List<QMS_CLIENT>();
    public object QueueGroup { get; set; }
}