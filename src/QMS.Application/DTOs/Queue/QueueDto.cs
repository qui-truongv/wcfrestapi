namespace QMS.Application.DTOs.Queue;

public class QueueDto
{
    public int ID { get; set; }
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
    public int IS_MANUAL { get; set; }
    public int? IDX { get; set; }
    public string? CODESCREEN { get; set; }
    public string? SectionName { get; set; }
    public string? QueueTypeName { get; set; }
}