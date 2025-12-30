namespace QMS.Core.Entities;

public abstract class BaseEntity
{
    public int ID { get; set; }
    public string? BENHVIEN_ID { get; set; }
    public DateTime NGAYTAO { get; set; } = DateTime.Now;
    public int? NGUOITAO_ID { get; set; }
    public DateTime? NGAYCAPNHAT { get; set; }
    public int? NGUOICAPNHAT_ID { get; set; }
}