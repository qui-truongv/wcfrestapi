namespace QMS.Core.Entities;

public class QMS_SCREEN : BaseEntity
{
    public string CODE { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public int? SECTION_ID { get; set; }
    public int? STATE { get; set; }
    public int? DISPLAYROWS { get; set; }
    public string? URL { get; set; }
    public string? REMARKS { get; set; }
    public int NUMSCREEN { get; set; } = 1;

    // Navigation Properties
    public virtual QMS_SECTION? Section { get; set; }
    public virtual ICollection<QMS_QUEUE> Queues { get; set; } = new List<QMS_QUEUE>();
}