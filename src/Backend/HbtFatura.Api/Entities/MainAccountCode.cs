namespace HbtFatura.Api.Entities;

/// <summary>Ana cari kodu (hesap planı). FirmId null = sistem kodu (THP, salt okunur); dolu = firma özel kodu.</summary>
public class MainAccountCode
{
    public Guid Id { get; set; }
    public Guid? FirmId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }

    public Firm? Firm { get; set; }
}
