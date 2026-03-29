using System.Globalization;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;

namespace HbtFatura.Api.Services;

/// <summary>Lookup tablosundan (sistem tanımlamaları) rapor ve API için etiket çözümü.</summary>
public static class LookupMaps
{
    /// <summary>Code alanı tam sayı olan gruplar: InvoiceStatus, OrderStatus, DeliveryNoteStatus, …</summary>
    public static async Task<Dictionary<int, string>> LoadIntCodeMapAsync(AppDbContext db, string lookupGroupName, CancellationToken ct)
    {
        var rows = await db.Lookups
            .AsNoTracking()
            .Where(x => x.Group != null && x.Group.Name == lookupGroupName && x.IsActive)
            .Select(x => new { x.Code, x.Name })
            .ToListAsync(ct);

        var map = new Dictionary<int, string>();
        foreach (var r in rows)
        {
            if (int.TryParse((r.Code ?? string.Empty).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var code))
                map[code] = r.Name;
        }

        return map;
    }

    /// <summary>Code alanı serbest metin: ProductUnit (Adet, Kg, …), Currency (TRY), …</summary>
    public static async Task<Dictionary<string, string>> LoadStringCodeMapAsync(AppDbContext db, string lookupGroupName, CancellationToken ct)
    {
        var rows = await db.Lookups
            .AsNoTracking()
            .Where(x => x.Group != null && x.Group.Name == lookupGroupName && x.IsActive)
            .Select(x => new { x.Code, x.Name })
            .ToListAsync(ct);

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in rows)
        {
            var k = (r.Code ?? string.Empty).Trim();
            if (k.Length > 0)
                map[k] = r.Name;
        }

        return map;
    }

    public static string FormatIntCode(int code, IReadOnlyDictionary<int, string> labels)
    {
        return labels.TryGetValue(code, out var name) ? name : code.ToString(CultureInfo.InvariantCulture);
    }

    public static string FormatStringCode(string? code, IReadOnlyDictionary<string, string> labels)
    {
        if (string.IsNullOrWhiteSpace(code)) return string.Empty;
        var k = code.Trim();
        return labels.TryGetValue(k, out var name) ? name : k;
    }
}
