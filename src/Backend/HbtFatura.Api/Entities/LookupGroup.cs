namespace HbtFatura.Api.Entities;

public class LookupGroup
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;        // e.g. "OrderType" (sistem anahtarı)
    public string DisplayName { get; set; } = string.Empty; // e.g. "Sipariş Tipi" (Türkçe)
    /// <summary>Grup başlığı İngilizce; boşsa UI Türkçe DisplayName kullanır.</summary>
    public string? DisplayNameEn { get; set; }
    public string? Description { get; set; }
    public bool IsSystemGroup { get; set; } = false;        // If true, maybe prevent deletion
    
    public ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
}
