using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Translation;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/translations")]
public class TranslationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TranslationsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>ngx-translate paketi — giriş sayfası dahil JWT gerektirmez.</summary>
    [HttpGet("{culture:regex(^(tr|en)$)}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBundle(string culture, CancellationToken ct)
    {
        var pairs = await _db.UiTranslations.AsNoTracking()
            .Where(x => x.Culture == culture)
            .Select(x => new { x.Key, x.Value })
            .ToListAsync(ct);

        var nested = TranslationJsonHelper.ToNested(pairs.Select(p => (p.Key, p.Value)));
        return Ok(nested);
    }

    /// <summary>Admin: benzersiz anahtar başına TR + EN yan yana (sayfalama anahtar sayısına göre).</summary>
    [HttpGet("admin")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<UiTranslationPairListResponse>> AdminListPairs(
        [FromQuery] string? q,
        [FromQuery] string? prefix,
        [FromQuery] string? order,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100,
        CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 500);
        skip = Math.Max(0, skip);
        var sortByKeyOnly = string.Equals(order, "key", StringComparison.OrdinalIgnoreCase);

        IQueryable<UiTranslation> filtered = _db.UiTranslations.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var qq = q.Trim();
            filtered = filtered.Where(x => x.Key.Contains(qq) || x.Value.Contains(qq));
        }

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            var p = prefix.Trim();
            if (string.Equals(p, "__flat", StringComparison.Ordinal))
                filtered = filtered.Where(x => !x.Key.Contains('.'));
            else
                filtered = filtered.Where(x => x.Key == p || x.Key.StartsWith(p + "."));
        }

        var keysQuery = filtered.Select(x => x.Key).Distinct();
        var allKeys = await keysQuery.ToListAsync(ct);
        var total = allKeys.Count;
        var sortedKeys = sortByKeyOnly
            ? allKeys.OrderBy(k => k).ToList()
            : allKeys.OrderBy(TranslationKeyPrefix).ThenBy(k => k).ToList();
        var pageKeys = sortedKeys.Skip(skip).Take(take).ToList();

        if (pageKeys.Count == 0)
            return Ok(new UiTranslationPairListResponse(Array.Empty<UiTranslationPairAdminDto>(), total));

        var rows = await _db.UiTranslations.AsNoTracking()
            .Where(x => pageKeys.Contains(x.Key))
            .ToListAsync(ct);

        var byKey = rows.GroupBy(x => x.Key).ToDictionary(g => g.Key, g => g.ToList());
        var items = new List<UiTranslationPairAdminDto>(pageKeys.Count);
        foreach (var key in pageKeys)
        {
            byKey.TryGetValue(key, out var list);
            list ??= new List<UiTranslation>();
            var tr = list.FirstOrDefault(x => x.Culture == "tr");
            var en = list.FirstOrDefault(x => x.Culture == "en");
            items.Add(new UiTranslationPairAdminDto(
                key,
                tr?.Id,
                tr?.Value ?? string.Empty,
                en?.Id,
                en?.Value ?? string.Empty));
        }

        return Ok(new UiTranslationPairListResponse(items, total));
    }

    [HttpPut("admin/pair")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> AdminUpdatePair([FromBody] UpdateUiTranslationPairRequest body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Key))
            return BadRequest("Key gerekli.");

        var key = body.Key.Trim();

        async Task UpsertAsync(string culture, string value, CancellationToken c)
        {
            var row = await _db.UiTranslations.FirstOrDefaultAsync(x => x.Key == key && x.Culture == culture, c);
            if (row == null)
            {
                _db.UiTranslations.Add(new UiTranslation
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Culture = culture,
                    Value = value
                });
            }
            else
            {
                row.Value = value;
            }
        }

        await UpsertAsync("tr", body.ValueTr ?? string.Empty, ct);
        await UpsertAsync("en", body.ValueEn ?? string.Empty, ct);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("admin/{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> AdminUpdate(Guid id, [FromBody] UpdateUiTranslationRequest body, CancellationToken ct)
    {
        var row = await _db.UiTranslations.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (row == null)
            return NotFound();

        row.Value = body?.Value ?? string.Empty;
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>İlk noktadan önceki segment; nokta yoksa tek grupta sıralamak için sabit önek.</summary>
    private static string TranslationKeyPrefix(string key)
    {
        if (string.IsNullOrEmpty(key))
            return "\u0001";
        var i = key.IndexOf('.');
        return i < 0 ? "\u0001" : key[..i];
    }
}
