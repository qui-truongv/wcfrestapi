namespace QMS.Core.Entities;

public class QMS_QUEUE_ITEM : BaseEntity
{
    public int? QUEUE_ID { get; set; }
    public int? SEQUENCE { get; set; }
    public string? PREFIX { get; set; }
    public string? DISPLAYTEXT { get; set; }
    public string? SUFFIX { get; set; }
    public string? ORDER { get; set; }
    public int? STATE { get; set; }
    public int? PRIORITY { get; set; }
    public int? PREVIOUS { get; set; }
    public int? PREVIOUSDEPT_ID { get; set; }
    public string? PATIENTCODE { get; set; }
    public string? PATIENTNAME { get; set; }
    public int? PATIENTYOB { get; set; }
    public DateTime? CREATEDATE { get; set; }
    public DateTime? CREATETIME { get; set; }
    public DateTime? ESTIMATETIME { get; set; }
    public DateTime? PROCESSTIME { get; set; }
    public DateTime? FINISHTIME { get; set; }
    public int? CLIENT_ID { get; set; }
    public string? CLIENT_NAME { get; set; }
    public string? REMARKS { get; set; }
    public int? ISMATOATHUOC { get; set; }
    public decimal? SOTIEN { get; set; }
    public string? TENCUA { get; set; }

    // Navigation Properties
    public virtual QMS_QUEUE? Queue { get; set; }
}