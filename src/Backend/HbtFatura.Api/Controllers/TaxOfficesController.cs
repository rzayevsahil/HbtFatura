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
    public async Task<ActionResult<IEnumerable<string>>> GetCities()
    {
        var cities = await _context.Cities
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync();
        return Ok(cities);
    }

    [HttpGet("districts/{city}")]
    public async Task<ActionResult<IEnumerable<string>>> GetDistricts(string city)
    {
        var districts = await _context.Districts
            .Where(x => x.City.Name == city)
            .OrderBy(x => x.Name)
            .Select(x => x.Name)
            .ToListAsync();
        return Ok(districts);
    }

    [HttpGet("offices/{city}/{district}")]
    public async Task<ActionResult<IEnumerable<TaxOfficeDto>>> GetOffices(string city, string district)
    {
        var offices = await _context.TaxOffices
            .Where(x => x.City.Name == city && x.District.Name == district)
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
