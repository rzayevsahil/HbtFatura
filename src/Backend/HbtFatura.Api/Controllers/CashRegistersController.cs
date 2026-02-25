using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.Constants;
using HbtFatura.Api.DTOs.Cash;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CashRegistersController : ControllerBase
{
    private readonly ICashRegisterService _service;

    public CashRegistersController(ICashRegisterService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CashRegisterDto>>> GetAll([FromQuery] Guid? firmId, CancellationToken ct)
    {
        var list = await _service.GetAllAsync(firmId, ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CashRegisterDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<CashRegisterDto>> Create([FromBody] CreateCashRegisterRequest request, CancellationToken ct)
    {
        var dto = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CashRegisterDto>> Update(Guid id, [FromBody] UpdateCashRegisterRequest request, CancellationToken ct)
    {
        var dto = await _service.UpdateAsync(id, request, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<PagedResult<CashTransactionDto>>> GetTransactions(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetTransactionsAsync(id, page, pageSize, dateFrom, dateTo, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/transactions")]
    public async Task<ActionResult<CashTransactionDto>> AddTransaction(Guid id, [FromBody] CreateCashTransactionRequest request, CancellationToken ct)
    {
        var dto = await _service.AddTransactionAsync(id, request, ct);
        return CreatedAtAction(nameof(GetTransactions), new { id }, dto);
    }
}
