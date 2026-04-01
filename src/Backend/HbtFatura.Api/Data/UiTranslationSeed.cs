using HbtFatura.Api.Entities;
using HbtFatura.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Data;

public static class UiTranslationSeed
{
    /// <summary>İlk kurulum: tablo boşsa JSON’dan toplu aktarır.</summary>
    public static async Task ImportIfEmptyAsync(AppDbContext db, string contentRootPath, CancellationToken ct = default)
    {
        if (await db.UiTranslations.AnyAsync(ct))
            return;

        var rows = await ReadSeedRowsAsync(contentRootPath, ct);
        if (rows.Count == 0)
            return;

        await db.UiTranslations.AddRangeAsync(rows, ct);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Tabloda zaten veri varken JSON’a sonradan eklenen anahtarları ekler (örn. <c>translationsPage.colTr</c>).
    /// Mevcut satırların metnini güncellemez; yalnızca eksik (Key, Culture) çifti eklenir.
    /// </summary>
    public static async Task MergeMissingFromSeedAsync(AppDbContext db, string contentRootPath, CancellationToken ct = default)
    {
        var incoming = await ReadSeedRowsAsync(contentRootPath, ct);
        if (incoming.Count == 0)
            return;

        var existingKeys = await db.UiTranslations
            .AsNoTracking()
            .Select(x => new { x.Key, x.Culture })
            .ToListAsync(ct);
        var existing = existingKeys.Select(x => (x.Key, x.Culture)).ToHashSet();

        var toAdd = incoming.Where(x => !existing.Contains((x.Key, x.Culture))).ToList();
        if (toAdd.Count == 0)
            return;

        await db.UiTranslations.AddRangeAsync(toAdd, ct);
        await db.SaveChangesAsync(ct);
    }

    private static async Task<List<UiTranslation>> ReadSeedRowsAsync(string contentRootPath, CancellationToken ct)
    {
        var dir = Path.Combine(contentRootPath, "Data", "SeedData", "i18n");
        var trPath = Path.Combine(dir, "tr.json");
        var enPath = Path.Combine(dir, "en.json");
        if (!File.Exists(trPath) || !File.Exists(enPath))
            return new List<UiTranslation>();

        var trJson = await File.ReadAllTextAsync(trPath, ct);
        var enJson = await File.ReadAllTextAsync(enPath, ct);

        var rows = new List<UiTranslation>();
        rows.AddRange(TranslationJsonHelper.Flatten("tr", trJson));
        rows.AddRange(TranslationJsonHelper.Flatten("en", enJson));
        return rows;
    }
}
