namespace QMS.Core.Entities;

public class QMS_KIOSK : BaseEntity
{
    public string CODE { get; set; } = string.Empty;
    public string? NAME { get; set; }
    public string? COMPUTERNAME { get; set; }
    public string? IPADDRESS { get; set; }
    public string? REMARKS { get; set; }

    // Navigation Properties
    public virtual ICollection<QMS_KIOSK_QUEUE> KioskQueues { get; set; } = new List<QMS_KIOSK_QUEUE>();
}