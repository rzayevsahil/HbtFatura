using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.DTOs.Product;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductListDto>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] Guid? firmId = null, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, firmId, ct);
        return Ok(result);
    }

    [HttpGet("dropdown")]
    public async Task<ActionResult<List<ProductDto>>> GetDropdown([FromQuery] Guid? firmId, CancellationToken ct = default)
    {
        var list = await _service.GetListForDropdownAsync(firmId, ct);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id, CancellationToken ct = default)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request, CancellationToken ct = default)
    {
        var dto = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct = default)
    {
        var dto = await _service.UpdateAsync(id, request, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpGet("{id:guid}/movements")]
    public async Task<ActionResult<PagedResult<StockMovementDto>>> GetMovements(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, CancellationToken ct = default)
    {
        var result = await _service.GetMovementsAsync(id, page, pageSize, dateFrom, dateTo, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/movements")]
    public async Task<ActionResult<StockMovementDto>> AddMovement(Guid id, [FromBody] CreateStockMovementRequest request, CancellationToken ct = default)
    {
        var dto = await _service.AddMovementAsync(id, request, ct);
        return CreatedAtAction(nameof(GetMovements), new { id }, dto);
    }
}
