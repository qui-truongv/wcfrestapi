namespace QMS.Application.DTOs.Screen;

public class ScreenQueueDisplayDto
{
    public int ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public List<QueueDisplayItemDto> Queues { get; set; } = new();
}