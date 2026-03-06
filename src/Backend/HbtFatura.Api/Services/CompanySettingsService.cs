using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.CompanySettings;
using HbtFatura.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace HbtFatura.Api.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IWebHostEnvironment _env;

    public CompanySettingsService(AppDbContext db, ICurrentUserContext currentUser, IWebHostEnvironment env)
    {
        _db = db;
        _currentUser = currentUser;
        _env = env;
    }

    public async Task<CompanySettingsDto?> GetByFirmIdAsync(Guid? firmId, CancellationToken ct = default)
    {
        var effectiveFirmId = firmId ?? _currentUser.FirmId;
        if (!effectiveFirmId.HasValue)
            return null;
        var entity = await _db.CompanySettings
            .Include(x => x.TaxOffice)
                .ThenInclude(t => t!.City)
            .Include(x => x.TaxOffice)
                .ThenInclude(t => t!.District)
            .FirstOrDefaultAsync(x => x.FirmId == effectiveFirmId.Value, ct);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<CompanySettingsDto?> CreateOrUpdateAsync(UpdateCompanySettingsRequest request, Guid? firmId, CancellationToken ct = default)
    {
        if (_currentUser.IsEmployee)
            return null;
        var effectiveFirmId = firmId ?? _currentUser.FirmId;
        if (!effectiveFirmId.HasValue && !_currentUser.IsSuperAdmin)
            return null;
        if (effectiveFirmId.HasValue && _currentUser.IsFirmAdmin && effectiveFirmId != _currentUser.FirmId)
            return null;

        if (!effectiveFirmId.HasValue)
            return null;

        var entity = await _db.CompanySettings.FirstOrDefaultAsync(x => x.FirmId == effectiveFirmId.Value, ct);
        if (entity == null)
        {
            entity = new CompanySettings
            {
                Id = Guid.NewGuid(),
                FirmId = effectiveFirmId.Value,
                CreatedAt = DateTime.UtcNow
            };
            _db.CompanySettings.Add(entity);
        }
        else
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
        entity.CompanyName = request.CompanyName.Trim();
        entity.TaxOfficeId = request.TaxOfficeId;
        entity.TaxNumber = request.TaxNumber?.Trim();
        entity.Address = request.Address?.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Email = request.Email?.Trim();
        entity.Website = request.Website?.Trim();
        entity.IBAN = request.IBAN?.Trim();
        entity.BankName = request.BankName?.Trim();
        
        // Logo handling: save as file if it's base64
        if (!string.IsNullOrEmpty(request.LogoUrl) && request.LogoUrl.StartsWith("data:image"))
        {
            var folderPath = Path.Combine(_env.WebRootPath, "uploads", "logos");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // Clean up old logo if exists
            if (!string.IsNullOrEmpty(entity.LogoUrl) && entity.LogoUrl.StartsWith("/uploads/"))
            {
                var oldPath = Path.Combine(_env.WebRootPath, entity.LogoUrl.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            var extension = ".png"; // Default to png or extract from data type
            if (request.LogoUrl.Contains("image/jpeg")) extension = ".jpg";
            else if (request.LogoUrl.Contains("image/webp")) extension = ".webp";

            var fileName = $"{effectiveFirmId.Value}_{DateTime.Now.Ticks}{extension}";
            var filePath = Path.Combine(folderPath, fileName);
            
            var base64Data = request.LogoUrl.Split(',')[1];
            var bytes = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(filePath, bytes, ct);
            
            entity.LogoUrl = $"/uploads/logos/{fileName}";
        }
        else if (string.IsNullOrEmpty(request.LogoUrl))
        {
            // Clean up if logo is removed
            if (!string.IsNullOrEmpty(entity.LogoUrl) && entity.LogoUrl.StartsWith("/uploads/"))
            {
                var oldPath = Path.Combine(_env.WebRootPath, entity.LogoUrl.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }
            entity.LogoUrl = null;
        }

        await _db.SaveChangesAsync(ct);
        
        // Final return with navigation names
        return await GetByFirmIdAsync(effectiveFirmId, ct);
    }

    private static CompanySettingsDto MapToDto(CompanySettings e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        CityId = e.TaxOffice?.CityId,
        CityName = e.TaxOffice?.City?.Name,
        DistrictId = e.TaxOffice?.DistrictId,
        DistrictName = e.TaxOffice?.District?.Name,
        TaxOfficeId = e.TaxOfficeId,
        TaxOfficeName = e.TaxOffice?.Name,
        TaxNumber = e.TaxNumber,
        Address = e.Address,
        Phone = e.Phone,
        Email = e.Email,
        Website = e.Website,
        IBAN = e.IBAN,
        BankName = e.BankName,
        LogoUrl = e.LogoUrl
    };
}
