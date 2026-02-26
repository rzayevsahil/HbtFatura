using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.DeliveryNotes;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeliveryNotesController : ControllerBase
{
    private readonly IDeliveryNoteService _service;

    public DeliveryNotesController(IDeliveryNoteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<DeliveryNoteListDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] DeliveryNoteStatus? status = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? orderId = null,
        [FromQuery] Guid? firmId = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetPagedAsync(page, pageSize, dateFrom, dateTo, status, customerId, orderId, firmId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryNoteDto>> Get(Guid id, CancellationToken ct = default)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryNoteDto>> Create([FromBody] CreateDeliveryNoteRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("from-order")]
    public async Task<ActionResult<DeliveryNoteDto>> CreateFromOrder([FromBody] CreateDeliveryNoteFromOrderRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.CreateFromOrderAsync(request.OrderId, request.DeliveryDate, ct);
            if (dto == null) return NotFound();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DeliveryNoteDto>> Update(Guid id, [FromBody] UpdateDeliveryNoteRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.UpdateAsync(id, request, ct);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> SetStatus(Guid id, [FromBody] SetDeliveryNoteStatusRequest request, CancellationToken ct = default)
    {
        var ok = await _service.SetStatusAsync(id, request.Status, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
