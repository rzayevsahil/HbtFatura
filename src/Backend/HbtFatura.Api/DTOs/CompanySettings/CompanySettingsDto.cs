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
    /// <summary>Fatura numarası seri (3 karakter). Boşsa firma adının ilk 3 harfi kullanılır.</summary>
    public string? InvoiceSerialPrefix { get; set; }
    /// <summary>İrsaliye numarası seri (3 karakter). Boşsa firma adının ilk 3 harfi kullanılır.</summary>
    public string? DeliveryNoteSerialPrefix { get; set; }
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
    /// <summary>Fatura seri (3 harf, opsiyonel). Örn: FTR.</summary>
    public string? InvoiceSerialPrefix { get; set; }
    /// <summary>İrsaliye seri (3 harf, opsiyonel). Örn: IRS.</summary>
    public string? DeliveryNoteSerialPrefix { get; set; }
}
