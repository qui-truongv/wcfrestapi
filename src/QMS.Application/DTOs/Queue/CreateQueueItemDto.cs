namespace QMS.Application.DTOs.Queue;

public class CreateQueueItemDto
{
    public string? PATIENTCODE { get; set; }
    public string? PATIENTNAME { get; set; }
    public int? PATIENTYOB { get; set; }
    public int DEPARTMENT_ID { get; set; }
    public int? QUEUE_ID { get; set; }
    public int PRIORITY { get; set; }
    public int STATE { get; set; }
    public int? CLIENT_ID { get; set; }
    public string? CLIENT_NAME { get; set; }
    public DateTime? NGAYCAPSTT { get; set; }
    public int? ISMATOATHUOC { get; set; }
    public decimal? SOTIEN { get; set; }
    public string? TENCUA { get; set; }
}