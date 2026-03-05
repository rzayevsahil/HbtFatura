using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.TaxOffice;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxOfficesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaxOfficesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("cities")]
    public async Task<ActionResult<IEnumerable<object>>> GetCities()
    {
        var cities = await _context.Cities
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(cities);
    }

    [HttpGet("districts/{cityId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetDistricts(Guid cityId)
    {
        var districts = await _context.Districts
            .Where(x => x.CityId == cityId)
            .OrderBy(x => x.Name)
            .Select(x => new { x.Id, x.Name })
            .ToListAsync();
        return Ok(districts);
    }

    [HttpGet("offices/{cityId}/{districtId}")]
    public async Task<ActionResult<IEnumerable<TaxOfficeDto>>> GetOffices(Guid cityId, Guid districtId)
    {
        var offices = await _context.TaxOffices
            .Where(x => x.CityId == cityId && x.DistrictId == districtId)
            .Select(x => new TaxOfficeDto
            {
                Id = x.Id,
                City = x.City.Name,
                District = x.District.Name,
                Name = x.Name
            })
            .OrderBy(x => x.Name)
            .ToListAsync();
        return Ok(offices);
    }
}
