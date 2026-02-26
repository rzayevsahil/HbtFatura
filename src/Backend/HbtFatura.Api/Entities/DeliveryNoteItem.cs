namespace HbtFatura.Api.Entities;

public class DeliveryNoteItem
{
    public Guid Id { get; set; }
    public Guid DeliveryNoteId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? OrderItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal VatRate { get; set; }
    public int SortOrder { get; set; }

    public DeliveryNote DeliveryNote { get; set; } = null!;
    public Product? Product { get; set; }
    public OrderItem? OrderItem { get; set; }
}
