using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Permissions;
using HbtFatura.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HbtFatura.Api.Controllers;

[Authorize(Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public PermissionsController(AppDbContext db, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _db = db;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<PermissionDto>>> GetPermissions()
    {
        return await _db.Permissions
            .OrderBy(p => p.Group)
            .ThenBy(p => p.Code)
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Group = p.Group,
                Code = p.Code,
                Name = p.Name
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<PermissionDto>> CreatePermission([FromBody] PermissionDto dto)
    {
        if (await _db.Permissions.AnyAsync(p => p.Code == dto.Code))
            return BadRequest("Permission code already exists.");

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Group = dto.Group,
            Code = dto.Code,
            Name = dto.Name
        };

        _db.Permissions.Add(permission);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPermissions), new { id = permission.Id }, new PermissionDto
        {
            Id = permission.Id,
            Group = permission.Group,
            Code = permission.Code,
            Name = permission.Name
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] PermissionDto dto)
    {
        var permission = await _db.Permissions.FindAsync(id);
        if (permission == null) return NotFound();

        permission.Group = dto.Group;
        permission.Name = dto.Name;
        // Code should probably be immutable or updated carefully as it links to code/DB
        
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(Guid id)
    {
        var permission = await _db.Permissions.FindAsync(id);
        if (permission == null) return NotFound();

        // Check if used in RolePermissions
        if (await _db.RolePermissions.AnyAsync(rp => rp.PermissionId == id))
            return BadRequest("Permission is assigned to roles and cannot be deleted.");

        _db.Permissions.Remove(permission);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        return await _roleManager.Roles
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? ""
            })
            .ToListAsync();
    }

    [HttpGet("role/{roleId}")]
    public async Task<ActionResult<List<string>>> GetRolePermissions(Guid roleId)
    {
        return await _db.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Join(_db.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Code)
            .ToListAsync();
    }

    [HttpPost("role/{roleId}")]
    public async Task<IActionResult> UpdateRolePermissions(Guid roleId, [FromBody] UpdateRolePermissionsRequest request)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null) return NotFound("Role not found.");

        // SuperAdmin permissions cannot be modified (it has all) - Optional safeguard
        if (role.Name == "SuperAdmin") return BadRequest("SuperAdmin permissions are fixed.");

        var currentRPs = await _db.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _db.RolePermissions.RemoveRange(currentRPs);

        var requestedPerms = await _db.Permissions
            .Where(p => request.PermissionCodes.Contains(p.Code))
            .ToListAsync();

        foreach (var p in requestedPerms)
        {
            _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = p.Id });
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
