namespace HbtFatura.Api.Entities;

public class CompanySettings
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public Guid? TaxOfficeId { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? IBAN { get; set; }
    public string? BankName { get; set; }
    public string? LogoUrl { get; set; }
    /// <summary>Fatura numarası seri (3 karakter, örn: FTR). Boşsa firma adından türetilir veya varsayılan kullanılır.</summary>
    public string? InvoiceSerialPrefix { get; set; }
    /// <summary>İrsaliye numarası seri (3 karakter, örn: IRS). Boşsa firma adından türetilir veya varsayılan kullanılır.</summary>
    public string? DeliveryNoteSerialPrefix { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
    public TaxOffice? TaxOffice { get; set; }
}
