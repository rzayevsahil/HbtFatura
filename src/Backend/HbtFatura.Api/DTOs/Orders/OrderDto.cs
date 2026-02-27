using HbtFatura.Api.Entities;

namespace HbtFatura.Api.DTOs.Orders;

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string? CustomerTitle { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public InvoiceType OrderType { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? DeliveryNoteId { get; set; }
    public string? DeliveryNoteNumber { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderListDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public InvoiceType OrderType { get; set; }
    public string? CustomerTitle { get; set; }
    public decimal? TotalAmount { get; set; }
}

public class OrderItemInputDto
{
    public Guid? ProductId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }
}

public class CreateOrderRequest
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public InvoiceType OrderType { get; set; } = InvoiceType.Satis;
    public List<OrderItemInputDto> Items { get; set; } = new();
}

public class UpdateOrderRequest
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    /// <summary>Yalnızca Bekliyor iken değiştirilebilir; sadece Bekliyor (0) veya Onaylandı (3) atanabilir.</summary>
    public OrderStatus? Status { get; set; }
    public List<OrderItemInputDto> Items { get; set; } = new();
}
