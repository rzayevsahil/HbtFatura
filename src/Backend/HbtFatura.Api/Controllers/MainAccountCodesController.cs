using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.MainAccountCode;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MainAccountCodesController : ControllerBase
{
    private readonly IMainAccountCodeService _service;

    public MainAccountCodesController(IMainAccountCodeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MainAccountCodeDto>>> GetAll([FromQuery] Guid? firmId, CancellationToken ct = default)
    {
        var list = await _service.GetByFirmAsync(firmId, ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MainAccountCodeDto>> Get(Guid id, CancellationToken ct = default)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<MainAccountCodeDto>> Create([FromBody] CreateMainAccountCodeRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MainAccountCodeDto>> Update(Guid id, [FromBody] UpdateMainAccountCodeRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, ct);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("Sistem kodları"))
                return StatusCode(403, new { message = ex.Message });
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("Sistem kodları"))
                return StatusCode(403, new { message = ex.Message });
            return BadRequest(new { message = ex.Message });
        }
    }
}
