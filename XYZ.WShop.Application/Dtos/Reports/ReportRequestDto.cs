

namespace XYZ.WShop.Application.Dtos.Reports
{
    public class ReportRequestDto
    {
        public string ReportType { get; set; } // "sales", "inventory", "customers"
        public string TimeRange { get; set; } // "week", "month"
    }

    public class ReportResponseDto
    {
        public string Title { get; set; }
        public string[] Labels { get; set; }
        public decimal[] Data { get; set; }
        public string Total { get; set; }
        public string Change { get; set; }
    }
}
