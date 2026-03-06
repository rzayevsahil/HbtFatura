namespace HbtFatura.Api.DTOs.CompanySettings;

public class CompanySettingsDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public Guid? CityId { get; set; }
    public string? CityName { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public Guid? TaxOfficeId { get; set; }
    public string? TaxOfficeName { get; set; }
    public string? TaxNumber { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? IBAN { get; set; }
    public string? BankName { get; set; }
    public string? LogoUrl { get; set; }
}

public class UpdateCompanySettingsRequest
{
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
}
