namespace HbtFatura.Api.DTOs.TaxOffice;

public class TaxOfficeDto
{
    public Guid Id { get; set; }
    public string City { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Name { get; set; } = default!;
}
