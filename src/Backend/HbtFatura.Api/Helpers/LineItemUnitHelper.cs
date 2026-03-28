using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Helpers;

public static class LineItemUnitHelper
{
    /// <summary>Ürün varsa ürün birimi; yoksa istekteki birim; o da yoksa Adet.</summary>
    public static string Resolve(string? requestUnit, Product? product)
    {
        if (product != null && !string.IsNullOrWhiteSpace(product.Unit))
            return product.Unit.Trim();
        if (!string.IsNullOrWhiteSpace(requestUnit))
            return requestUnit.Trim();
        return "Adet";
    }
}
