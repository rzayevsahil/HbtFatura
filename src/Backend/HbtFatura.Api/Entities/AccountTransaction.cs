namespace HbtFatura.Api.Entities;

public class AccountTransaction
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; } // 1=Borç, 2=Alacak
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty; // Fatura, Tahsilat, Odeme, Manuel
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Customer Customer { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
