namespace HbtFatura.Api.Entities;

public class ChequeOrPromissory
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public int Type { get; set; } // 1=Çek, 2=Senet
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public int Status { get; set; } // Portföyde, TahsilEdildi, Ödendi, Reddedildi
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid? BankAccountId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public BankAccount? BankAccount { get; set; }
}
