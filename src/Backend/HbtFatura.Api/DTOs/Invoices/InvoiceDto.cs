using HbtFatura.Api.Entities;

namespace HbtFatura.Api.DTOs.Invoices;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalVat { get; set; }
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "TRY";
    public decimal ExchangeRate { get; set; }
    public List<InvoiceItemDto> Items { get; set; } = new();
}

public class InvoiceListDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Currency { get; set; } = "TRY";
}

public class CreateInvoiceRequest
{
    public DateTime InvoiceDate { get; set; }
    public Guid? CustomerId { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public string Currency { get; set; } = "TRY";
    public decimal ExchangeRate { get; set; } = 1;
    public List<InvoiceItemInputDto> Items { get; set; } = new();
}

public class UpdateInvoiceRequest : CreateInvoiceRequest { }
