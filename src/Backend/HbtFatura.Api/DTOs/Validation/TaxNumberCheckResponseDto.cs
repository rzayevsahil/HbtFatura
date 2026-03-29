namespace HbtFatura.Api.DTOs.Validation;

public class TaxNumberCheckResponseDto
{
    public bool IsValidFormat { get; set; }
    public string? Normalized { get; set; }
    public bool IsUnique { get; set; }
    public string? Message { get; set; }
}
