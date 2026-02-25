namespace HbtFatura.Api.Entities;

public class CashTransaction
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public DateTime Date { get; set; }
    public int Type { get; set; } // 1=Giriş, 2=Çıkış
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }

    public CashRegister CashRegister { get; set; } = null!;
}
