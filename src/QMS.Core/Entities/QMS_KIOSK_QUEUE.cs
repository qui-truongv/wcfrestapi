namespace QMS.Core.Entities;

public class QMS_KIOSK_QUEUE : BaseEntity
{
    public int KIOSK_ID { get; set; }
    public int? QUEUE_ID { get; set; }
    public string? DISPLAYTEXT { get; set; }
    public int? PRIORITY { get; set; }
    public string? REMARKS { get; set; }

    // Navigation Properties
    public virtual QMS_KIOSK Kiosk { get; set; } = null!;
    public virtual QMS_QUEUE? Queue { get; set; }
    public object DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}