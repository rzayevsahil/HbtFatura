using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Invoices;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _service;
    private readonly IInvoicePdfService _pdfService;

    public InvoicesController(IInvoiceService service, IInvoicePdfService pdfService)
    {
        _service = service;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<InvoiceListDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] InvoiceStatus? status = null,
        [FromQuery] InvoiceType? invoiceType = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? firmId = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetPagedAsync(page, pageSize, dateFrom, dateTo, status, invoiceType, customerId, firmId, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceRequest request, CancellationToken ct)
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

    [HttpPost("from-delivery-note")]
    public async Task<ActionResult<InvoiceDto>> CreateFromDeliveryNote([FromBody] CreateInvoiceFromDeliveryNoteRequest request, CancellationToken ct = default)
    {
        try
        {
            var dto = await _service.CreateFromDeliveryNoteAsync(request.DeliveryNoteId, ct);
            if (dto == null) return NotFound();
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> Update(Guid id, [FromBody] UpdateInvoiceRequest request, [FromHeader(Name = "If-Match")] string? ifMatch, CancellationToken ct = default)
    {
        byte[]? rowVersion = null;
        if (!string.IsNullOrEmpty(ifMatch))
        {
            try
            {
                rowVersion = Convert.FromBase64String(ifMatch);
            }
            catch { /* optional */ }
        }
        try
        {
            var dto = await _service.UpdateAsync(id, request, rowVersion, ct);
            if (dto == null) return NotFound();
            return Ok(dto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetStatusRequest req, CancellationToken ct)
    {
        var ok = await _service.SetStatusAsync(id, req.Status, ct);
        if (!ok) return BadRequest(new { message = "Cannot change status." });
        return NoContent();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken ct)
    {
        var pdf = await _pdfService.GeneratePdfAsync(id, ct);
        if (pdf == null) return NotFound();
        return File(pdf, "application/pdf", $"fatura-{id}.pdf");
    }
}

public class SetStatusRequest
{
    public InvoiceStatus Status { get; set; }
}

public class CreateInvoiceFromDeliveryNoteRequest
{
    public Guid DeliveryNoteId { get; set; }
}
