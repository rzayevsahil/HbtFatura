namespace HbtFatura.Api.DTOs.CompanySettings;

public class CompanySettingsDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IBAN { get; set; }
    public string? LogoUrl { get; set; }
}

public class UpdateCompanySettingsRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? IBAN { get; set; }
    public string? LogoUrl { get; set; }
}
