using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.CompanySettings;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompanySettingsController : ControllerBase
{
    private readonly ICompanySettingsService _service;

    public CompanySettingsController(ICompanySettingsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CompanySettingsDto>> Get([FromQuery] Guid? firmId, CancellationToken ct)
    {
        var dto = await _service.GetByFirmIdAsync(firmId, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPut]
    public async Task<ActionResult<CompanySettingsDto>> CreateOrUpdate([FromBody] UpdateCompanySettingsRequest request, [FromQuery] Guid? firmId, CancellationToken ct)
    {
        var dto = await _service.CreateOrUpdateAsync(request, firmId, ct);
        if (dto == null) return Forbid();
        return Ok(dto);
    }
}
