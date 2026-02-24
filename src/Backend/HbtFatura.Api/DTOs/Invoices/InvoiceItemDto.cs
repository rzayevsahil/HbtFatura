using HbtFatura.Api.Entities;

namespace HbtFatura.Api.DTOs.Invoices;

public class InvoiceItemDto
{
    public Guid? Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public decimal LineTotalExclVat { get; set; }
    public decimal LineVatAmount { get; set; }
    public decimal LineTotalInclVat { get; set; }
    public int SortOrder { get; set; }
}

public class InvoiceItemInputDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }
}
