namespace HbtFatura.Api.Entities;

public class Invoice
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid UserId { get; set; }
    public Guid? CustomerId { get; set; }

    // Customer snapshot (for display/print; filled from Customer or manual)
    public string CustomerTitle { get; set; } = string.Empty;
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public decimal SubTotal { get; set; }
    public decimal TotalVat { get; set; }
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "TRY";
    public decimal ExchangeRate { get; set; } = 1;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public byte[]? RowVersion { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Customer? Customer { get; set; }
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
}
