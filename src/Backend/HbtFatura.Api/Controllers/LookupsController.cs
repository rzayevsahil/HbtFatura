using System.Globalization;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;
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
    private readonly AppDbContext _db;
    public LookupsController(AppDbContext db) => _db = db;

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
            .Where(x => x.Group != null && x.Group.Name == "VatRate" && x.IsActive)
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
        if (lookup.Id == Guid.Empty) lookup.Id = Guid.NewGuid();
        _db.Lookups.Add(lookup);
        await _db.SaveChangesAsync();
        return Ok(lookup);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Update(Guid id, Lookup lookup)
    {
        var existing = await _db.Lookups.FindAsync(id);
        if (existing == null) return NotFound();

        existing.LookupGroupId = lookup.LookupGroupId;
        existing.Code = lookup.Code;
        existing.Name = lookup.Name;
        existing.Color = lookup.Color;
        existing.SortOrder = lookup.SortOrder;
        existing.IsActive = lookup.IsActive;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _db.Lookups.FindAsync(id);
        if (existing == null) return NotFound();

        _db.Lookups.Remove(existing);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
