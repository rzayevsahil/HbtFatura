namespace HbtFatura.Api.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    /// <summary>Tek düzen hesap planına göre ana cari kodu (örn. 120.01.001)</summary>
    public string? MainAccountCode { get; set; }
    /// <summary>Cari kodu (örn. A01 alıcı, S01 satıcı)</summary>
    public string? Code { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Mükellef türü: 1=Gerçek kişi, 2=Tüzel kişi</summary>
    public int TaxPayerType { get; set; } = 2;
    /// <summary>Cari kart tipi: 1=Alıcı, 2=Satıcı, 3=Alıcı+Satıcı</summary>
    public int CardType { get; set; } = 1;
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<AccountTransaction> AccountTransactions { get; set; } = new List<AccountTransaction>();
}
