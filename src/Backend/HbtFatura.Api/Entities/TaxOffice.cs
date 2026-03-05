using System.ComponentModel.DataAnnotations;

namespace HbtFatura.Api.Entities;

public class TaxOffice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string District { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
