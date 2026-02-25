using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<CustomerListDto>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] Guid? firmId = null, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetPagedAsync(page, pageSize, search, firmId, ct);
        return Ok(result);
    }

    [HttpGet("dropdown")]
    public async Task<ActionResult<List<CustomerDto>>> GetDropdown(CancellationToken ct)
    {
        var list = await _service.GetListForDropdownAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get(Guid id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpGet("{id:guid}/balance")]
    public async Task<ActionResult<decimal>> GetBalance(Guid id, CancellationToken ct)
    {
        var balance = await _service.GetBalanceAsync(id, ct);
        return Ok(balance);
    }

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<PagedResult<AccountTransactionDto>>> GetTransactions(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetTransactionsAsync(id, page, pageSize, dateFrom, dateTo, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CreateCustomerRequest request, CancellationToken ct)
    {
        var dto = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken ct)
    {
        var dto = await _service.UpdateAsync(id, request, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _service.SoftDeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}
