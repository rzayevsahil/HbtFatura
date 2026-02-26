namespace HbtFatura.Api.Entities;

/// <summary>Sipariş durumu: Bekliyor, Tamamı Teslim (irsaliyeye dönüştü), İptal.</summary>
public enum OrderStatus
{
    Bekliyor = 0,
    TamamiTeslim = 1,
    Iptal = 2
}
