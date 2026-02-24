using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.Constants;
using HbtFatura.Api.DTOs.Employees;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.FirmAdmin)]
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
    public async Task<ActionResult<EmployeeListDto>> Create([FromBody] CreateEmployeeRequest request, CancellationToken ct)
    {
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
}
