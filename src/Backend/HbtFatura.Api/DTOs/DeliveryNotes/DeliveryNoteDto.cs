using HbtFatura.Api.Entities;

namespace HbtFatura.Api.DTOs.DeliveryNotes;

public class DeliveryNoteItemDto
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public Guid? OrderItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }
}

public class DeliveryNoteDto
{
    public Guid Id { get; set; }
    public string DeliveryNumber { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public Guid? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? InvoiceId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DeliveryNoteStatus Status { get; set; }
    public InvoiceType DeliveryType { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<DeliveryNoteItemDto> Items { get; set; } = new();
}

public class DeliveryNoteListDto
{
    public Guid Id { get; set; }
    public string DeliveryNumber { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public DeliveryNoteStatus Status { get; set; }
    public InvoiceType DeliveryType { get; set; }
    public string? CustomerTitle { get; set; }
    public string? OrderNumber { get; set; }
    public Guid? InvoiceId { get; set; }
}

public class DeliveryNoteItemInputDto
{
    public Guid? ProductId { get; set; }
    public Guid? OrderItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }
}

public class CreateDeliveryNoteRequest
{
    public Guid? CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public InvoiceType DeliveryType { get; set; } = InvoiceType.Satis;
    public List<DeliveryNoteItemInputDto> Items { get; set; } = new();
}

public class CreateDeliveryNoteFromOrderRequest
{
    public Guid OrderId { get; set; }
    public DateTime DeliveryDate { get; set; }
}

public class UpdateDeliveryNoteRequest
{
    public Guid? CustomerId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public List<DeliveryNoteItemInputDto> Items { get; set; } = new();
}

public class SetDeliveryNoteStatusRequest
{
    public DeliveryNoteStatus Status { get; set; }
}
