using System.ComponentModel.DataAnnotations;

namespace HbtFatura.Api.Entities;

public class TaxOffice
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CityId { get; set; }
    public City City { get; set; } = default!;

    [Required]
    public Guid DistrictId { get; set; }
    public District District { get; set; } = default!;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
