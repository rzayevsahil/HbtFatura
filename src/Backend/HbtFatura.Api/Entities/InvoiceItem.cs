namespace HbtFatura.Api.Entities;

public class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    /// <summary>İskonto yüzdesi (0-100).</summary>
    public decimal DiscountPercent { get; set; }

    // Calculated by backend (Quantity * UnitPrice, then VAT)
    public decimal LineTotalExclVat { get; set; }
    public decimal LineVatAmount { get; set; }
    public decimal LineTotalInclVat { get; set; }
    public int SortOrder { get; set; }
    public Guid? ProductId { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Product? Product { get; set; }
}
