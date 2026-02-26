namespace HbtFatura.Api.Entities;

/// <summary>İrsaliye durumu: Taslak, Onaylandı (stok hareketi oluşturuldu), İptal.</summary>
public enum DeliveryNoteStatus
{
    Taslak = 0,
    Onaylandi = 1,
    Iptal = 2,
    Faturalandi = 3
}
