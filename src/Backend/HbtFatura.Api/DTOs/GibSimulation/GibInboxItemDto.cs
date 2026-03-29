namespace HbtFatura.Api.DTOs.GibSimulation;

public class GibInboxItemDto
{
    public Guid SubmissionId { get; set; }
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string SenderFirmName { get; set; } = string.Empty;
    public string CustomerTitle { get; set; } = string.Empty;
    public string RecipientTaxNumber { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
