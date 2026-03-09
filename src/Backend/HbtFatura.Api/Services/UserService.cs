using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.DTOs.Users;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Data;

namespace HbtFatura.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;
    private readonly ILogService _log;

    public UserService(UserManager<ApplicationUser> userManager, AppDbContext db, ILogService log)
    {
        _userManager = userManager;
        _db = db;
        _log = log;
    }

    public async Task<UserProfileDto?> GetProfileAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var roleName = roles.FirstOrDefault() ?? "";
        
        var roleDisplayName = await _db.Roles
            .Where(r => r.Name == roleName)
            .Select(r => r.DisplayName)
            .FirstOrDefaultAsync(ct);

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Role = roleName,
            RoleDisplayName = roleDisplayName
        };
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return false;
        
        await _log.LogAsync("Profil bilgileri güncellendi", "UpdateProfile", "User", "Info", $"UserId: {userId}, Ad Soyad: {user.FullName}");

        if (!string.IsNullOrEmpty(request.CurrentPassword) && !string.IsNullOrEmpty(request.NewPassword))
        {
            var pwdResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!pwdResult.Succeeded) return false;
            await _log.LogAsync("Kullanıcı şifresi değiştirildi", "ChangePassword", "User", "Info", $"UserId: {userId}");
        }

        return true;
    }
}
