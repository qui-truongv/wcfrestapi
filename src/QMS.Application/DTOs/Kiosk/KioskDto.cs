namespace QMS.Application.DTOs.Kiosk;

public class KioskDto
{
    public int ID { get; set; }
    public string CODE { get; set; } = string.Empty;
    public string? NAME { get; set; }
    public string? COMPUTERNAME { get; set; }
    public string? IPADDRESS { get; set; }
    public string? REMARKS { get; set; }
    public DateTime NGAYTAO { get; set; }
    public DateTime? NGAYCAPNHAT { get; set; }
}