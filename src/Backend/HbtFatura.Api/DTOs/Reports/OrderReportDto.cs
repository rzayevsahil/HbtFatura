namespace HbtFatura.Api.DTOs.Reports;

public class OrderReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public int? Status { get; set; }
    public string? Search { get; set; }
    public Guid? FirmId { get; set; }
    public List<OrderReportRowDto> Items { get; set; } = new();
}

public class OrderReportRowDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int Status { get; set; }
    public int OrderType { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
}

