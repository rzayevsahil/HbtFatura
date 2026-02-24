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

    public async Task<IReadOnlyList<EmployeeListDto>> GetByFirmAsync(CancellationToken ct = default)
    {
        if (!_currentUser.FirmId.HasValue)
            return Array.Empty<EmployeeListDto>();
        var roleId = await _db.Roles.Where(r => r.Name == Roles.Employee).Select(r => r.Id).FirstOrDefaultAsync(ct);
        if (roleId == default)
            return Array.Empty<EmployeeListDto>();

        var employeeUserIds = await _db.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(ct);

        return await _db.Users
            .Where(u => u.FirmId == _currentUser.FirmId && employeeUserIds.Contains(u.Id))
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
        if (!_currentUser.IsFirmAdmin || !_currentUser.FirmId.HasValue)
            throw new UnauthorizedAccessException("Only FirmAdmin can add employees.");

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
            FirmId = _currentUser.FirmId.Value
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
}
