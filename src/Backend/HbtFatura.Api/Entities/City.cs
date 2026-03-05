namespace HbtFatura.Api.Entities;

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public ICollection<District> Districts { get; set; } = new List<District>();
}
