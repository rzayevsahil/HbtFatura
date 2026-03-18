namespace HbtFatura.Api.DTOs.Reports;

public class MonthlyProductSalesReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<MonthlyProductSalesRowDto> Items { get; set; } = new();
}

public class MonthlyProductSalesRowDto
{
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal QuantitySold { get; set; }
}
