using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Validation;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/tax-numbers")]
[Authorize]
public class TaxNumbersController : ControllerBase
{
    private readonly ITaxNumberUniquenessService _uniqueness;
    private readonly ICurrentUserContext _currentUser;

    public TaxNumbersController(ITaxNumberUniquenessService uniqueness, ICurrentUserContext currentUser)
    {
        _uniqueness = uniqueness;
        _currentUser = currentUser;
    }

    /// <summary>
    /// TC/VKN tekilliği (cari veya şirket kaydı). mode: customer | company
    /// </summary>
    [HttpGet("check")]
    public async Task<ActionResult<TaxNumberCheckResponseDto>> Check(
        [FromQuery] string? value,
        [FromQuery] string mode = "customer",
        [FromQuery] Guid? excludeCustomerId = null,
        [FromQuery] Guid? firmId = null,
        CancellationToken ct = default)
    {
        if (!Enum.TryParse<TaxNumberCheckMode>(mode, ignoreCase: true, out var checkMode))
            return BadRequest(new { message = "Geçersiz mode (customer veya company)." });

        Guid? excludeFirm = null;
        if (checkMode == TaxNumberCheckMode.Company)
        {
            var effectiveFirm = firmId ?? _currentUser.FirmId;
            if (!effectiveFirm.HasValue)
                return BadRequest(new { message = "Şirket vergisi kontrolü için firma bilgisi gerekli." });
            if (!_currentUser.IsSuperAdmin && _currentUser.FirmId != effectiveFirm)
                return Forbid();
            excludeFirm = effectiveFirm;
        }

        var result = await _uniqueness.CheckAsync(value, checkMode, excludeCustomerId, excludeFirm, ct);
        return Ok(result);
    }
}
