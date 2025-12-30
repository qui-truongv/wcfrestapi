namespace QMS.Application.DTOs.Client;

public class ClientDto
{
    public int ID { get; set; }
    public string NAME { get; set; } = string.Empty;
    public string? COMPUTERNAME { get; set; }
    public string? IPADDRESS { get; set; }
    public int? STATE { get; set; }
    public int? PROCESSDURATION { get; set; }
    public int? QUEUE_ID { get; set; }
    public string? REMARKS { get; set; }
    public string? IPSERVER { get; set; }
    public string? NAMEAUDIO { get; set; }
    public string? QueueName { get; set; }
}