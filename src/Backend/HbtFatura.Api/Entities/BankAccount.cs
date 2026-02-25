namespace HbtFatura.Api.Entities;

public class BankAccount
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public string Currency { get; set; } = "TRY";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
    public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
}
