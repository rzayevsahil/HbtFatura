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
    private readonly ILogService _log;

    public EmployeeService(AppDbContext db, UserManager<ApplicationUser> userManager, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _userManager = userManager;
        _currentUser = currentUser;
        _log = log;
    }

    /// <summary>Firma kullanıcıları (çalışan düzenleme). SuperAdmin tüm firmalar; diğer roller kendi firması.</summary>
    private IQueryable<ApplicationUser> ScopeQuery()
    {
        var baseQuery = _db.Users.AsQueryable();
        if (_currentUser.IsSuperAdmin)
            return baseQuery.Where(u => u.FirmId != null);
        if (!_currentUser.FirmId.HasValue)
            return baseQuery.Where(u => false);
        return baseQuery.Where(u => u.FirmId == _currentUser.FirmId);
    }

    public async Task<IReadOnlyList<EmployeeListDto>> GetByFirmAsync(CancellationToken ct = default)
    {
        if (!await _currentUser.HasPermissionAsync("Employees.View", _db, ct))
            return Array.Empty<EmployeeListDto>();

        // SuperAdmin: genel personel listesi yok; liste firma detayında Firmalar/{id}/users ile.
        if (_currentUser.IsSuperAdmin)
            return Array.Empty<EmployeeListDto>();

        return await ScopeQuery()
            .OrderBy(u => u.FullName)
            .Select(u => new EmployeeListDto
            {
                Id = u.Id,
                FirmId = u.FirmId,
                Email = u.Email ?? "",
                FullName = u.FullName,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<EmployeeListDto> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        if (!await _currentUser.HasPermissionAsync("Employees.Edit", _db, ct))
            throw new UnauthorizedAccessException("Personel oluşturma yetkiniz bulunmamaktadır.");

        if (!_currentUser.FirmId.HasValue)
            throw new ArgumentException("FirmId is required.");
        var effectiveFirmId = _currentUser.FirmId.Value;

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
            FirmId = effectiveFirmId
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));
        await _userManager.AddToRoleAsync(user, Roles.Employee);

        await _log.LogAsync($"Personel oluşturuldu: {user.FullName} ({user.Email})", "Create", "Employee", "Info", $"Id: {user.Id}, FirmId: {effectiveFirmId}");

        return new EmployeeListDto
        {
            Id = user.Id,
            FirmId = user.FirmId,
            Email = user.Email ?? "",
            FullName = user.FullName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<EmployeeListDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (!await _currentUser.HasPermissionAsync("Employees.View", _db, ct))
            return null;

        return await ScopeQuery()
            .Where(u => u.Id == id)
            .Select(u => new EmployeeListDto
            {
                Id = u.Id,
                FirmId = u.FirmId,
                Email = u.Email ?? "",
                FullName = u.FullName,
                CreatedAt = u.CreatedAt
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<EmployeeListDto> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct = default)
    {
        if (!await _currentUser.HasPermissionAsync("Employees.Edit", _db, ct))
            throw new UnauthorizedAccessException("Personel düzenleme yetkiniz bulunmamaktadır.");

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

        await _log.LogAsync($"Personel güncellendi: {user.FullName} ({user.Email})", "Update", "Employee", "Info", $"Id: {user.Id}");

        return new EmployeeListDto
        {
            Id = user.Id,
            FirmId = user.FirmId,
            Email = user.Email ?? "",
            FullName = user.FullName,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (!await _currentUser.HasPermissionAsync("Employees.Edit", _db, ct))
            throw new UnauthorizedAccessException("Personel silme yetkiniz bulunmamaktadır.");

        var user = await ScopeQuery().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null)
            throw new ArgumentException("Çalışan bulunamadı.");

        var email = user.Email ?? "";
        var fullName = user.FullName ?? "";
        var firmId = user.FirmId;

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await _log.LogAsync($"Personel silindi: {fullName} ({email})", "Delete", "Employee", "Warning", $"Id: {id}, FirmId: {firmId}");
    }
}
