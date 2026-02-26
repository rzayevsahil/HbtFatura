namespace HbtFatura.Api.DTOs.Reports;

public class InvoiceReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public List<InvoiceReportRowDto> Items { get; set; } = new();
}

public class InvoiceReportRowDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public int Status { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "TRY";
}
