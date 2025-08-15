

namespace XYZ.WShop.Application.Dtos.Marketing
{
    public class LinkAnalyticsResponse
    {
        public SummaryMetrics Summary { get; set; }
        public List<ProductMetrics> Products { get; set; }
        public List<CountryMetrics> Countries { get; set; }
        public string DateRange { get; set; }
    }

    public class SummaryMetrics
    {
        public int TotalViews { get; set; }
        public int TotalCatalogViews { get; set; }
        public int WhatsAppClicks { get; set; }
        public string CTR { get; set; }
    }

    public class ProductMetrics
    {
        public string Name { get; set; }
        public int Views { get; set; }
        public int WhatsAppClicks { get; set; }
        public string CTR { get; set; }
    }

    public class CountryMetrics
    {
        public string Name { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
    }
}
