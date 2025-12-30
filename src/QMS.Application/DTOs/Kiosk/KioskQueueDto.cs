namespace QMS.Application.DTOs.Kiosk;

public class KioskQueueDto
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public int ScreenId { get; set; }
    public int Priority { get; set; }
    public int DisplayOrder { get; set; }
}