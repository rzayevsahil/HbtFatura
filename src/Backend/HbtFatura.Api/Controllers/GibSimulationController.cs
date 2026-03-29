using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GibSimulationController : ControllerBase
{
    private readonly IGibSimulationService _gib;

    public GibSimulationController(IGibSimulationService gib)
    {
        _gib = gib;
    }

    [HttpGet("inbox")]
    [Authorize(Policy = "GibSimulation.ViewInbox")]
    public async Task<IActionResult> Inbox(CancellationToken ct)
    {
        var items = await _gib.GetInboxAsync(ct);
        return Ok(items);
    }

    [HttpPost("{id:guid}/accept")]
    [Authorize(Policy = "GibSimulation.Accept")]
    public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
    {
        try
        {
            await _gib.AcceptAsync(id, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "GibSimulation.Accept")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
    {
        try
        {
            await _gib.RejectAsync(id, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
