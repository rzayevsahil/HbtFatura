namespace HbtFatura.Api.Entities;

public class District
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;

    public City City { get; set; } = null!;
}
