using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.AccountPayment;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/account-payments")]
[Authorize]
public class AccountPaymentsController : ControllerBase
{
    private readonly IAccountPaymentService _service;

    public AccountPaymentsController(IAccountPaymentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AccountPaymentListDto>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] string? type = null,
        [FromQuery] Guid? firmId = null,
        CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;
        var result = await _service.GetPagedAsync(page, pageSize, dateFrom, dateTo, customerId, type, firmId, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] AccountPaymentRequest request, CancellationToken ct)
    {
        await _service.CreateAsync(request, ct);
        return NoContent();
    }
}
