namespace HbtFatura.Api.Entities;

public class Firm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public CompanySettings? CompanySettings { get; set; }
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
