namespace HbtFatura.Api.Entities;

public class CompanySettings
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IBAN { get; set; }
    public string? BankName { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
}
