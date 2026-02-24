using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Firms;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class FirmService : IFirmService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserContext _currentUser;

    public FirmService(AppDbContext db, UserManager<ApplicationUser> userManager, ICurrentUserContext currentUser)
    {
        _db = db;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<FirmDto>> GetAllAsync(CancellationToken ct = default)
    {
        if (!_currentUser.IsSuperAdmin)
            return Array.Empty<FirmDto>();
        return await _db.Firms
            .OrderBy(x => x.Name)
            .Select(x => new FirmDto { Id = x.Id, Name = x.Name, CreatedAt = x.CreatedAt })
            .ToListAsync(ct);
    }

    public async Task<FirmDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (!_currentUser.IsSuperAdmin)
            return null;
        var firm = await _db.Firms.FindAsync(new object[] { id }, ct);
        return firm == null ? null : new FirmDto { Id = firm.Id, Name = firm.Name, CreatedAt = firm.CreatedAt };
    }

    public async Task<FirmDto> CreateAsync(CreateFirmRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.IsSuperAdmin)
            throw new UnauthorizedAccessException("Only SuperAdmin can create firms.");

        var email = request.AdminEmail.Trim().ToLowerInvariant();
        if (await _userManager.FindByEmailAsync(email) != null)
            throw new ArgumentException("Admin email already registered.");

        var firm = new Firm
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _db.Firms.Add(firm);

        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = request.AdminFullName.Trim(),
            CreatedAt = DateTime.UtcNow,
            FirmId = firm.Id
        };
        var result = await _userManager.CreateAsync(adminUser, request.AdminPassword);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));
        await _userManager.AddToRoleAsync(adminUser, Roles.FirmAdmin);

        var companySettings = new CompanySettings
        {
            Id = Guid.NewGuid(),
            FirmId = firm.Id,
            CompanyName = firm.Name,
            CreatedAt = DateTime.UtcNow
        };
        _db.CompanySettings.Add(companySettings);

        await _db.SaveChangesAsync(ct);

        return new FirmDto { Id = firm.Id, Name = firm.Name, CreatedAt = firm.CreatedAt };
    }

    public async Task<FirmDto?> UpdateAsync(Guid id, UpdateFirmRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.IsSuperAdmin)
            return null;
        var firm = await _db.Firms.FindAsync(new object[] { id }, ct);
        if (firm == null) return null;
        firm.Name = request.Name.Trim();
        await _db.SaveChangesAsync(ct);
        return new FirmDto { Id = firm.Id, Name = firm.Name, CreatedAt = firm.CreatedAt };
    }
}
