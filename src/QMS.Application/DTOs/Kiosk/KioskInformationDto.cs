namespace QMS.Application.DTOs.Kiosk;

public class KioskInformationDto
{
    public string KioskName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public List<KioskQueueDto> Queues { get; set; } = new();
}