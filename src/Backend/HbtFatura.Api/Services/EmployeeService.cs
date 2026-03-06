using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Employees;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserContext _currentUser;

    public EmployeeService(AppDbContext db, UserManager<ApplicationUser> userManager, ICurrentUserContext currentUser)
    {
        _db = db;
        _userManager = userManager;
        _currentUser = currentUser;
    }

    private IQueryable<ApplicationUser> ScopeQuery(Guid? firmIdFilter = null)
    {
        var roleIdQuery = _db.Roles.Where(r => r.Name == Roles.Employee).Select(r => r.Id);
        var baseQuery = _db.Users
            .Join(_db.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
            .Where(x => roleIdQuery.Contains(x.ur.RoleId))
            .Select(x => x.u);

        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return baseQuery.Where(u => u.FirmId == firmIdFilter.Value);
            return baseQuery;
        }

        return baseQuery.Where(u => u.FirmId == _currentUser.FirmId);
    }

    public async Task<IReadOnlyList<EmployeeListDto>> GetByFirmAsync(CancellationToken ct = default)
    {
        return await ScopeQuery()
            .OrderBy(u => u.FullName)
            .Select(u => new EmployeeListDto
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<EmployeeListDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.IsFirmAdmin && !_currentUser.IsSuperAdmin)
            throw new UnauthorizedAccessException("Unauthorized to create employees.");

        var effectiveFirmId = _currentUser.IsSuperAdmin ? (request.FirmId ?? _currentUser.FirmId) : _currentUser.FirmId;
        if (!effectiveFirmId.HasValue)
            throw new ArgumentException("FirmId is required.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await _userManager.FindByEmailAsync(email) != null)
            throw new ArgumentException("Email already registered.");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            CreatedAt = DateTime.UtcNow,
            FirmId = effectiveFirmId.Value
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));
        await _userManager.AddToRoleAsync(user, Roles.Employee);

        return new EmployeeListDto
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<EmployeeListDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await ScopeQuery()
            .Where(u => u.Id == id)
            .Select(u => new EmployeeListDto
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<EmployeeListDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct = default)
    {
        var user = await ScopeQuery().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            throw new ArgumentException("Çalışan bulunamadı.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (user.Email != email)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null && existing.Id != user.Id)
                throw new ArgumentException("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
            
            user.Email = email;
            user.UserName = email;
        }

        user.FullName = request.FullName.Trim();
        
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!passResult.Succeeded)
                throw new ArgumentException("Şifre güncellenemedi: " + string.Join(" ", passResult.Errors.Select(e => e.Description)));
        }

        return new EmployeeListDto
        {
            Id = user.Id,
            Email = user.Email ?? "",
            FullName = user.FullName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await ScopeQuery().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            throw new ArgumentException("Çalışan bulunamadı.");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));
    }
}
