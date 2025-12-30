namespace QMS.Core.Entities;

public class QMS_PARAMETER : BaseEntity
{
    public string CODE { get; set; } = string.Empty;
    public string? NAME { get; set; }
    public string? VALUE { get; set; }
    public string? REMARKS { get; set; }
}