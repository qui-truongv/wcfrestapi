namespace QMS.Application.DTOs.Screen;

public class ScreenDto
{
    public int ID { get; set; }
    public string CODE { get; set; } = string.Empty;
    public string NAME { get; set; } = string.Empty;
    public int? SECTION_ID { get; set; }
    public int? STATE { get; set; }
    public int? DISPLAYROWS { get; set; }
    public string? URL { get; set; }
    public string? REMARKS { get; set; }
    public int NUMSCREEN { get; set; }
    public string? SectionName { get; set; }
    public DateTime NGAYTAO { get; set; }
}