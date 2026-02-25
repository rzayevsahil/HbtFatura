using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Cheque;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/cheque-or-promissories")]
[Authorize]
public class ChequeOrPromissoriesController : ControllerBase
{
    private readonly IChequeOrPromissoryService _service;

    public ChequeOrPromissoriesController(IChequeOrPromissoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ChequeOrPromissoryDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? type = null,
        [FromQuery] int? status = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? firmId = null,
        [FromQuery] DateTime? dueFrom = null,
        [FromQuery] DateTime? dueTo = null,
        CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, type, status, customerId, firmId, dueFrom, dueTo, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ChequeOrPromissoryDto>> Get(Guid id, CancellationToken ct = default)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ChequeOrPromissoryDto>> Create([FromBody] CreateChequeOrPromissoryRequest request, CancellationToken ct = default)
    {
        var dto = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ChequeOrPromissoryDto>> Update(Guid id, [FromBody] UpdateChequeOrPromissoryRequest request, CancellationToken ct = default)
    {
        var dto = await _service.UpdateAsync(id, request, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> SetStatus(Guid id, [FromBody] SetChequeStatusRequest request, CancellationToken ct = default)
    {
        var ok = await _service.SetStatusAsync(id, request.Status, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
