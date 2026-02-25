using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.AccountPayment;
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

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] AccountPaymentRequest request, CancellationToken ct)
    {
        await _service.CreateAsync(request, ct);
        return NoContent();
    }
}
