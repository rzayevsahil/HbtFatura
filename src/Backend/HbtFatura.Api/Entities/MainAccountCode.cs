namespace HbtFatura.Api.Entities;

/// <summary>Ana cari kodu (hesap planı) - firma bazlı kod listesi.</summary>
public class MainAccountCode
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    public Firm Firm { get; set; } = null!;
}
