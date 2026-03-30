using System.Security.Claims;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Menu;
using HbtFatura.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogService _log;

    public MenuController(AppDbContext db, ILogService log)
    {
        _db = db;
        _log = log;
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuDto>>> GetMenu()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        List<string> userPermissions = new();
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            userPermissions = await (from ur in _db.UserRoles
                                     join rp in _db.RolePermissions on ur.RoleId equals rp.RoleId
                                     join p in _db.Permissions on rp.PermissionId equals p.Id
                                     where ur.UserId == userId
                                     select p.Code).ToListAsync();
        }

        var allMenus = await _db.Menus
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .ToListAsync();

        var permittedMenus = allMenus
            .Where(m => string.IsNullOrEmpty(m.RequiredPermissionCode) || userPermissions.Contains(m.RequiredPermissionCode))
            .ToList();

        // Build hierarchy
        var result = permittedMenus
            .Where(m => m.ParentId == null)
            .OrderBy(m => m.SortOrder)
            .Select(m => MapToDto(m, permittedMenus))
            .ToList();

        return result;
    }

    [HttpGet("all")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<MenuDto>>> GetAllMenus()
    {
        var allMenus = await _db.Menus.OrderBy(m => m.SortOrder).ToListAsync();
        return allMenus.Select(m => MapToDto(m, allMenus)).ToList();
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<MenuDto>> CreateMenu([FromBody] MenuDto dto)
    {
        var menu = new Entities.Menu
        {
            Id = Guid.NewGuid(),
            ParentId = dto.ParentId,
            Label = dto.Label,
            Icon = dto.Icon,
            RouterLink = dto.RouterLink,
            SortOrder = dto.SortOrder,
            RequiredPermissionCode = dto.RequiredPermissionCode,
            IsActive = true, // Default to true
            IsSystemMenu = false
        };

        _db.Menus.Add(menu);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Menü öğesi oluşturuldu: {menu.Label}", "Create", "Menu", "Info", $"Id: {menu.Id}, RouterLink: {menu.RouterLink}");

        return MapToDto(menu, new List<Entities.Menu>());
    }

    /// <summary>Sürükle-bırak sıralama: çoklu menü için parent ve sıra numarasını tek işlemde günceller.</summary>
    [HttpPut("reorder")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> ReorderMenus([FromBody] List<MenuReorderItemDto> items, CancellationToken ct)
    {
        if (items == null || items.Count == 0) return BadRequest(new { message = "Empty payload." });

        foreach (var dto in items)
        {
            var menu = await _db.Menus.FirstOrDefaultAsync(m => m.Id == dto.Id, ct);
            if (menu == null) continue;
            menu.ParentId = dto.ParentId;
            menu.SortOrder = dto.SortOrder;
        }

        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Menü sıralaması güncellendi ({items.Count} kayıt)", "Reorder", "Menu", "Info", null);
        return NoContent();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateMenu(Guid id, [FromBody] MenuDto dto)
    {
        var menu = await _db.Menus.FindAsync(id);
        if (menu == null) return NotFound();

        if (menu.IsSystemMenu)
        {
             // Optional: protect some fields for system menus
        }

        menu.ParentId = dto.ParentId;
        menu.Label = dto.Label;
        menu.Icon = dto.Icon;
        menu.RouterLink = dto.RouterLink;
        menu.SortOrder = dto.SortOrder;
        menu.RequiredPermissionCode = dto.RequiredPermissionCode;
        menu.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();
        await _log.LogAsync($"Menü öğesi güncellendi: {menu.Label}", "Update", "Menu", "Info", $"Id: {id}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeleteMenu(Guid id)
    {
        var menu = await _db.Menus.FindAsync(id);
        if (menu == null) return NotFound();
        if (menu.IsSystemMenu) return BadRequest("System menus cannot be deleted.");

        var label = menu.Label;
        _db.Menus.Remove(menu);
        await _db.SaveChangesAsync();
        await _log.LogAsync($"Menü öğesi silindi: {label}", "Delete", "Menu", "Warning", $"Id: {id}");
        return NoContent();
    }

    private MenuDto MapToDto(Entities.Menu m, List<Entities.Menu> all)
    {
        return new MenuDto
        {
            Id = m.Id,
            ParentId = m.ParentId,
            Label = m.Label,
            Icon = m.Icon,
            RouterLink = m.RouterLink,
            SortOrder = m.SortOrder,
            RequiredPermissionCode = m.RequiredPermissionCode,
            IsActive = m.IsActive,
            IsSystemMenu = m.IsSystemMenu,
            Children = all
                .Where(x => x.ParentId == m.Id)
                .OrderBy(x => x.SortOrder)
                .Select(x => MapToDto(x, all))
                .ToList()
        };
    }
}
