using System.Globalization;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupsController : ControllerBase
{
    private const string VatRateGroupName = "VatRate";

    private readonly AppDbContext _db;
    private readonly ILogService _log;

    public LookupsController(AppDbContext db, ILogService log)
    {
        _db = db;
        _log = log;
    }

    private static bool IsValidVatRateCode(string? code) =>
        !string.IsNullOrWhiteSpace(code)
        && decimal.TryParse(code, NumberStyles.Any, CultureInfo.InvariantCulture, out var p)
        && p is >= 0 and <= 100;

    private static string VatRateDisplayNameFromCode(string code) => $"%{code.Trim()}";

    // --- Lookup Groups ---

    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups()
    {
        var groups = await _db.LookupGroups
            .OrderBy(x => x.DisplayName)
            .ToListAsync();
        return Ok(groups);
    }

    [HttpPost("groups")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> CreateGroup(LookupGroup group)
    {
        if (group.Id == Guid.Empty) group.Id = Guid.NewGuid();
        _db.LookupGroups.Add(group);
        await _db.SaveChangesAsync();
        return Ok(group);
    }

    // --- Lookups ---

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var lookups = await _db.Lookups
            .Include(x => x.Group)
            .OrderBy(x => x.Group != null ? x.Group.Name : string.Empty)
            .ThenBy(x => x.SortOrder)
            .ToListAsync();
        return Ok(lookups);
    }

    [HttpGet("default-vat-rate")]
    public async Task<IActionResult> GetDefaultVatRate([FromServices] IConfiguration config)
    {
        var code = await _db.Lookups
            .AsNoTracking()
            .Include(x => x.Group)
            .Where(x => x.Group != null && x.Group.Name == VatRateGroupName && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.Code)
            .FirstOrDefaultAsync();

        if (!string.IsNullOrWhiteSpace(code)
            && decimal.TryParse(code, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            && parsed is >= 0 and <= 100)
            return Ok(new { defaultVatRate = parsed });

        var fallback = config.GetValue("App:DefaultVatRate", 20);
        if (fallback is < 0 or > 100)
            fallback = 20;
        return Ok(new { defaultVatRate = fallback });
    }

    [HttpGet("{groupName}")]
    public async Task<IActionResult> GetByGroup(string groupName)
    {
        var lookups = await _db.Lookups
            .Include(x => x.Group)
            .Where(x => x.Group!.Name == groupName && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();
        return Ok(lookups);
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create(Lookup lookup)
    {
        var group = await _db.LookupGroups.AsNoTracking().FirstOrDefaultAsync(g => g.Id == lookup.LookupGroupId);
        if (group == null)
            return BadRequest(new { message = "Geçersiz grup." });

        if (group.Name == VatRateGroupName)
        {
            var count = await _db.Lookups.CountAsync(x => x.LookupGroupId == lookup.LookupGroupId);
            if (count >= 1)
                return BadRequest(new { message = "KDV oranı için yalnızca tek tanım olabilir. Mevcut kaydı düzenleyin." });

            if (!IsValidVatRateCode(lookup.Code))
                return BadRequest(new { message = "KDV kodu 0 ile 100 arasında geçerli bir sayı olmalıdır (örn. 20)." });

            lookup.Code = lookup.Code.Trim();
            lookup.Name = VatRateDisplayNameFromCode(lookup.Code);
        }

        if (lookup.Id == Guid.Empty) lookup.Id = Guid.NewGuid();
        _db.Lookups.Add(lookup);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Lookup oluşturuldu: {lookup.Code} — {lookup.Name}", "Create", "Lookup", "Info", $"Id: {lookup.Id}, GrupId: {lookup.LookupGroupId}");
        return Ok(lookup);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, Lookup lookup)
    {
        var existing = await _db.Lookups.Include(x => x.Group).FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return NotFound();

        if (existing.Group?.Name == VatRateGroupName)
        {
            if (!IsValidVatRateCode(lookup.Code))
                return BadRequest(new { message = "KDV kodu 0 ile 100 arasında geçerli bir sayı olmalıdır (örn. 20)." });

            existing.Code = lookup.Code.Trim();
            existing.Name = VatRateDisplayNameFromCode(existing.Code);
            existing.Color = lookup.Color;
            existing.IsActive = true;
            // Grup, sıra ve pasifleştirme değiştirilemez — tek sistem KDV satırı
            await _db.SaveChangesAsync();
            await _log.LogAsync($"KDV oranı (lookup) güncellendi: {existing.Code}", "Update", "Lookup", "Info", $"Id: {id}");
            return Ok(existing);
        }

        existing.LookupGroupId = lookup.LookupGroupId;
        existing.Code = lookup.Code;
        existing.Name = lookup.Name;
        existing.Color = lookup.Color;
        existing.SortOrder = lookup.SortOrder;
        existing.IsActive = lookup.IsActive;

        await _db.SaveChangesAsync();
        await _log.LogAsync($"Lookup güncellendi: {existing.Code} — {existing.Name}", "Update", "Lookup", "Info", $"Id: {id}");
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Lookups.Include(x => x.Group).FirstOrDefaultAsync(x => x.Id == id);
        if (existing == null) return NotFound();

        if (existing.Group?.Name == VatRateGroupName)
            return BadRequest(new { message = "KDV oranı tanımı silinemez; yalnızca oran değerini güncelleyebilirsiniz." });

        var code = existing.Code;
        var name = existing.Name;
        _db.Lookups.Remove(existing);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Lookup silindi: {code} — {name}", "Delete", "Lookup", "Warning", $"Id: {id}");
        return Ok();
    }
}
