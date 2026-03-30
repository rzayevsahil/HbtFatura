using System.Text.RegularExpressions;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.MaterialIcon;
using HbtFatura.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/material-icons")]
[Authorize]
public class MaterialIconsController : ControllerBase
{
    private static readonly Regex LigaturePattern = new(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

    private readonly AppDbContext _db;

    public MaterialIconsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>İkon seçici: yalnızca aktif ligature adları.</summary>
    [HttpGet("for-picker")]
    public async Task<ActionResult<IReadOnlyList<string>>> ForPicker(CancellationToken ct)
    {
        var names = await _db.MaterialIconOptions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.LigatureName)
            .Select(x => x.LigatureName)
            .ToListAsync(ct);
        return Ok(names);
    }

    [HttpGet]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<List<MaterialIconDto>>> GetAll(CancellationToken ct)
    {
        var list = await _db.MaterialIconOptions
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.LigatureName)
            .Select(x => new MaterialIconDto(x.Id, x.LigatureName, x.SortOrder, x.IsActive))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<MaterialIconDto>> Create([FromBody] CreateMaterialIconRequest request, CancellationToken ct)
    {
        var name = NormalizeLigature(request.LigatureName);
        if (name == null)
            return BadRequest("Geçersiz ligature adı (küçük harf, rakam ve alt çizgi; harf ile başlamalı).");

        if (await _db.MaterialIconOptions.AnyAsync(x => x.LigatureName == name, ct))
            return Conflict("Bu ligature zaten kayıtlı.");

        var entity = new MaterialIconOption
        {
            Id = Guid.NewGuid(),
            LigatureName = name,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.MaterialIconOptions.Add(entity);
        await _db.SaveChangesAsync(ct);
        return Ok(new MaterialIconDto(entity.Id, entity.LigatureName, entity.SortOrder, entity.IsActive));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaterialIconRequest request, CancellationToken ct)
    {
        var name = NormalizeLigature(request.LigatureName);
        if (name == null)
            return BadRequest("Geçersiz ligature adı.");

        var entity = await _db.MaterialIconOptions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        if (await _db.MaterialIconOptions.AnyAsync(x => x.LigatureName == name && x.Id != id, ct))
            return Conflict("Bu ligature başka bir kayıtta kullanılıyor.");

        entity.LigatureName = name;
        entity.SortOrder = request.SortOrder;
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(ct);
        return Ok(new MaterialIconDto(entity.Id, entity.LigatureName, entity.SortOrder, entity.IsActive));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _db.MaterialIconOptions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null)
            return NotFound();

        _db.MaterialIconOptions.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static string? NormalizeLigature(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;
        var s = raw.Trim().ToLowerInvariant();
        return LigaturePattern.IsMatch(s) ? s : null;
    }
}
