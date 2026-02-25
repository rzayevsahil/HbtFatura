namespace HbtFatura.Api.Entities;

public class CashRegister
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
    public ICollection<CashTransaction> Transactions { get; set; } = new List<CashTransaction>();
}
