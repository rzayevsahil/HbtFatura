namespace HbtFatura.Api.Entities;

public class DeliveryNote
{
    public Guid Id { get; set; }
    public string DeliveryNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    /// <summary>Faturaya aktarıldıysa oluşan fatura Id. Doluysa irsaliye düzenlenemez.</summary>
    public Guid? InvoiceId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DeliveryNoteStatus Status { get; set; } = DeliveryNoteStatus.Taslak;
    public InvoiceType DeliveryType { get; set; } = InvoiceType.Satis;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Order? Order { get; set; }
    public ICollection<DeliveryNoteItem> Items { get; set; } = new List<DeliveryNoteItem>();
}
