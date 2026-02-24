using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.Constants;
using HbtFatura.Api.DTOs.Firms;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.SuperAdmin)]
public class FirmsController : ControllerBase
{
    private readonly IFirmService _service;

    public FirmsController(IFirmService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<FirmDto>>> GetAll(CancellationToken ct)
    {
        var list = await _service.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FirmDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{id:guid}/users")]
    public async Task<ActionResult<IReadOnlyList<FirmUserDto>>> GetUsers(Guid id, CancellationToken ct)
    {
        var list = await _service.GetUsersByFirmIdAsync(id, ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<FirmDto>> Create([FromBody] CreateFirmRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FirmDto>> Update(Guid id, [FromBody] UpdateFirmRequest request, CancellationToken ct)
    {
        var dto = await _service.UpdateAsync(id, request, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }
}
