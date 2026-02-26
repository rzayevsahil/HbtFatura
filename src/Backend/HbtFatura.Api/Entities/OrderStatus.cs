namespace HbtFatura.Api.Entities;

/// <summary>Sipariş durumu: Draft, Confirmed, PartiallyDelivered, FullyDelivered, Cancelled.</summary>
public enum OrderStatus
{
    /// <summary>Taslak, düzenlenebilir.</summary>
    Bekliyor = 0,
    /// <summary>Tamamı sevk edildi; düzenlenemez.</summary>
    TamamiTeslim = 1,
    /// <summary>İptal.</summary>
    Iptal = 2,
    /// <summary>Onaylandı.</summary>
    Onaylandi = 3,
    /// <summary>Kısmi sevk edildi; düzenlenemez.</summary>
    KismiTeslim = 4
}
