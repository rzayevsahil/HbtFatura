namespace HbtFatura.Api.Entities;

/// <summary>Menü / ikon seçici için Material+Icons ligature adları (klasik web font).</summary>
public class MaterialIconOption
{
    public Guid Id { get; set; }

    /// <summary>Örn. home, shopping_cart (benzersiz).</summary>
    public string LigatureName { get; set; } = string.Empty;

    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
