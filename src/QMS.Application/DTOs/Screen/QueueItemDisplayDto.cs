namespace QMS.Application.DTOs.Screen
{
    public class QueueItemDisplayDto
    {
        public int ID { get; set; }
        public int? SEQUENCE { get; set; }
        public string? DISPLAYTEXT { get; set; }
        public int? STATE { get; set; }
        public string? StateName { get; set; }
        public int? PRIORITY { get; set; }
        public string? PATIENTCODE { get; set; }
        public string? PATIENTNAME { get; set; }
        public int? PATIENTYOB { get; set; }
        public string? CLIENT_NAME { get; set; }
        public DateTime? CREATEDATE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public decimal? SOTIEN { get; set; }
        public string? TENCUA { get; set; }
        public string? TENBACSI { get; set; }
        public string? TENDIEUDUONG { get; set; }
    }
}
