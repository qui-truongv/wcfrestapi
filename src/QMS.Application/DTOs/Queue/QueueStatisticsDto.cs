namespace QMS.Application.DTOs.Queue;

public class QueueStatisticsDto
{
    public int Queue_ID { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public int Wait { get; set; }
    public int Process { get; set; }
    public int Done { get; set; }
    public int Miss { get; set; }
    public int Cancel { get; set; }
    public int Total { get; set; }
    public string? ProcessingSequence { get; set; }
}