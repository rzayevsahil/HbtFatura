namespace HbtFatura.Api.DTOs.Reports;

public class DeliveryNoteReportDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public int? Status { get; set; }
    public string? Search { get; set; }
    public Guid? FirmId { get; set; }
    public List<DeliveryNoteReportRowDto> Items { get; set; } = new();
}

public class DeliveryNoteReportRowDto
{
    public Guid Id { get; set; }
    public string DeliveryNumber { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public int Status { get; set; }
    public int DeliveryType { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? InvoiceId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
}

