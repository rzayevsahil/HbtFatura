using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.Constants;
using HbtFatura.Api.DTOs.Employees;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeListDto>>> GetAll(CancellationToken ct)
    {
        var list = await _service.GetByFirmAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = Roles.FirmAdmin + "," + Roles.SuperAdmin)]
    public async Task<ActionResult<EmployeeListDto>> Create([FromBody] CreateEmployeeRequest request, CancellationToken ct)
    {
        // ... handled in service but double check with attribute
        try
        {
            var dto = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetAll), null, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeListDto>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.FirmAdmin + "," + Roles.SuperAdmin)]
    public async Task<ActionResult<EmployeeListDto>> Update(Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, ct);
            return Ok(dto);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.FirmAdmin + "," + Roles.SuperAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
