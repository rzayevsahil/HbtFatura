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
public class RolesController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RolesController(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetRoles()
    {
        return await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? "",
                DisplayName = r.DisplayName,
                IsSystem = r.IsSystem
            })
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Role name is required.");
        
        if (await _roleManager.RoleExistsAsync(dto.Name))
            return BadRequest("Role already exists.");

        var role = new ApplicationRole(dto.Name) { DisplayName = dto.DisplayName, IsSystem = false };
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new RoleDto { Id = role.Id, Name = role.Name ?? "", DisplayName = role.DisplayName, IsSystem = role.IsSystem });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDto dto)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return NotFound();

        if (role.Name == "SuperAdmin" && dto.Name != "SuperAdmin") 
            return BadRequest("SuperAdmin role cannot be renamed.");

        role.Name = dto.Name;
        role.DisplayName = dto.DisplayName;
        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null) return NotFound();

        if (role.IsSystem) return BadRequest("System roles cannot be deleted.");

        // AspNetCore Identity handles foreign key checks or cascade if configured
        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return NoContent();
    }
}
