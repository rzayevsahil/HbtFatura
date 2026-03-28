using HbtFatura.Api.Constants;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Helpers;

/// <summary>
/// Stok hareketi yönü: alış belgelerinde mal girişi (+), satış belgelerinde çıkış (-).
/// Sipariş kayıtları bu kurallara göre stok miktarını değiştirmez.
/// </summary>
public static class InventoryStockMovementHelper
{
    public static int MovementTypeForDocument(InvoiceType documentType) =>
        documentType == InvoiceType.Alis ? StockMovementType.Giris : StockMovementType.Cikis;
}
