using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Auth;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Constants;

namespace HbtFatura.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    private readonly ICurrentUserContext _currentUser;

    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, IConfiguration config, ICurrentUserContext currentUser)
    {
        _db = db;
        _userManager = userManager;
        _config = config;
        _currentUser = currentUser;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, string? ipAddress, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var role = (request.Role ?? "").Trim();
        if (string.IsNullOrEmpty(role))
            throw new ArgumentException("Role is required (SuperAdmin, FirmAdmin, or Employee).");

        if (await _userManager.FindByEmailAsync(email) != null)
            throw new ArgumentException("Email already registered.");

        ApplicationUser user;
        string roleToAssign;

        if (string.Equals(role, Roles.SuperAdmin, StringComparison.OrdinalIgnoreCase))
        {
            if (await _db.Users.AnyAsync(ct))
                throw new UnauthorizedAccessException("SuperAdmin can only be created when no users exist (first-time setup).");
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                FirmId = null
            };
            roleToAssign = Roles.SuperAdmin;
        }
        else if (string.Equals(role, Roles.FirmAdmin, StringComparison.OrdinalIgnoreCase))
        {
            if (!_currentUser.IsSuperAdmin || _currentUser.UserId == Guid.Empty)
                throw new UnauthorizedAccessException("Only SuperAdmin can register FirmAdmin.");
            if (!request.FirmId.HasValue)
                throw new ArgumentException("FirmId is required for FirmAdmin.");
            var firmExists = await _db.Firms.AnyAsync(f => f.Id == request.FirmId.Value, ct);
            if (!firmExists)
                throw new ArgumentException("Firm not found.");
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                FirmId = request.FirmId.Value
            };
            roleToAssign = Roles.FirmAdmin;
        }
        else if (string.Equals(role, Roles.Employee, StringComparison.OrdinalIgnoreCase))
        {
            if (!_currentUser.IsFirmAdmin || !_currentUser.FirmId.HasValue)
                throw new UnauthorizedAccessException("Only FirmAdmin can register Employee.");
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FullName = request.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                FirmId = _currentUser.FirmId.Value
            };
            roleToAssign = Roles.Employee;
        }
        else
            throw new ArgumentException("Invalid role. Use SuperAdmin, FirmAdmin, or Employee.");

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new ArgumentException(string.Join(" ", result.Errors.Select(e => e.Description)));
        await _userManager.AddToRoleAsync(user, roleToAssign);

        return await BuildAuthResponseAsync(user, ipAddress, ct);
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.ToLowerInvariant());
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return null;

        return await BuildAuthResponseAsync(user, ipAddress, ct);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
    {
        var token = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshToken, ct);
        if (token == null || !token.IsActive)
            return null;

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _db.SaveChangesAsync(ct);

        return await BuildAuthResponseAsync(token.User, ipAddress, ct);
    }

    public async Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
    {
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, ct);
        if (token == null) return;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        await _db.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user, string? ipAddress, CancellationToken ct)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "";
        var userWithFirm = await _db.Users.Include(x => x.Firm).FirstOrDefaultAsync(x => x.Id == user.Id, ct);
        var firmName = userWithFirm?.Firm?.Name;

        var accessToken = GenerateAccessToken(user, role, firmName);
        var refreshTokenEntity = await CreateRefreshTokenAsync(user.Id, ipAddress, ct);
        var expiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenEntity.Token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                Role = role,
                FirmId = user.FirmId,
                FirmName = firmName
            }
        };
    }

    private string GenerateAccessToken(ApplicationUser user, string role, string? firmName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (user.FirmId.HasValue)
            claims.Add(new Claim("FirmId", user.FirmId.Value.ToString()));
        if (!string.IsNullOrEmpty(firmName))
            claims.Add(new Claim("FirmName", firmName));
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string? ipAddress, CancellationToken ct)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays()),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync(ct);
        return token;
    }

    private string GetJwtKey() => _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set");
    private int GetAccessTokenExpirationMinutes() => int.TryParse(_config["Jwt:AccessTokenExpirationMinutes"], out var m) ? m : 15;
    private int GetRefreshTokenExpirationDays() => int.TryParse(_config["Jwt:RefreshTokenExpirationDays"], out var d) ? d : 7;
}
