namespace HbtFatura.Api.Entities;

public class Firm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public CompanySettings? CompanySettings { get; set; }
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<CashRegister> CashRegisters { get; set; } = new List<CashRegister>();
    public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<ChequeOrPromissory> ChequeOrPromissories { get; set; } = new List<ChequeOrPromissory>();
}
