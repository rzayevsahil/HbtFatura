using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.CompanySettings;
using HbtFatura.Api.Entities;
using HbtFatura.Api.Helpers;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace HbtFatura.Api.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ITaxNumberUniquenessService _taxNumberUniqueness;

    public CompanySettingsService(AppDbContext db, ICurrentUserContext currentUser, IWebHostEnvironment env, ITaxNumberUniquenessService taxNumberUniqueness)
    {
        _db = db;
        _currentUser = currentUser;
        _env = env;
        _taxNumberUniqueness = taxNumberUniqueness;
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
        var companyName = (request.CompanyName ?? "").Trim();
        entity.CompanyName = companyName.Length > 0 ? companyName : (entity.CompanyName ?? "");
        entity.TaxOfficeId = request.TaxOfficeId;
        var taxNorm = TaxNumberNormalization.Normalize(request.TaxNumber);
        entity.TaxNumber = string.IsNullOrEmpty(taxNorm) ? null : taxNorm;
        entity.InvoiceSerialPrefix = NormalizeSerialPrefix(request.InvoiceSerialPrefix);
        entity.DeliveryNoteSerialPrefix = NormalizeSerialPrefix(request.DeliveryNoteSerialPrefix);
        entity.Address = request.Address?.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Email = request.Email?.Trim();
        entity.Website = request.Website?.Trim();
        entity.IBAN = request.IBAN?.Trim();
        entity.BankName = request.BankName?.Trim();
        
        // Logo handling: save as file if it's base64
        if (!string.IsNullOrEmpty(request.LogoUrl) && request.LogoUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            var webRoot = !string.IsNullOrEmpty(_env.WebRootPath)
                ? _env.WebRootPath
                : Path.Combine(_env.ContentRootPath, "wwwroot");
            var folderPath = Path.Combine(webRoot, "uploads", "logos");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            // Eski dosya kilitliyse (önizleme/IIS) silme patlayıp tüm kaydı düşürmesin — yeni dosya yine yazılır.
            if (!string.IsNullOrEmpty(entity.LogoUrl) && entity.LogoUrl.StartsWith("/uploads/"))
            {
                try
                {
                    var oldPath = Path.Combine(webRoot, entity.LogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
                catch
                {
                    /* Devam et */
                }
            }

            var extension = ".png";
            if (request.LogoUrl.Contains("image/jpeg", StringComparison.OrdinalIgnoreCase)) extension = ".jpg";
            else if (request.LogoUrl.Contains("image/webp", StringComparison.OrdinalIgnoreCase)) extension = ".webp";

            var fileName = $"{effectiveFirmId.Value}_{DateTime.Now.Ticks}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            var comma = request.LogoUrl.IndexOf(',');
            if (comma < 0) throw new InvalidOperationException("Geçersiz logo verisi.");
            var base64Data = request.LogoUrl[(comma + 1)..].Trim();
            var bytes = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(filePath, bytes, ct);

            entity.LogoUrl = $"/uploads/logos/{fileName}";
        }
        else if (string.IsNullOrEmpty(request.LogoUrl))
        {
            if (!string.IsNullOrEmpty(entity.LogoUrl) && entity.LogoUrl.StartsWith("/uploads/"))
            {
                var webRoot = !string.IsNullOrEmpty(_env.WebRootPath)
                    ? _env.WebRootPath
                    : Path.Combine(_env.ContentRootPath, "wwwroot");
                try
                {
                    var oldPath = Path.Combine(webRoot, entity.LogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
                catch { /* Logoyu veritabanından kaldırmaya devam et */ }
            }
            entity.LogoUrl = null;
        }

        await _taxNumberUniqueness.EnsureUniqueForCompanyAsync(entity.TaxNumber, effectiveFirmId.Value, ct);
        await _db.SaveChangesAsync(ct);
        
        // Final return with navigation names
        return await GetByFirmIdAsync(effectiveFirmId, ct);
    }

    /// <summary>Trims to max 3 chars, uppercase, normalizes Turkish to ASCII, keeps only A-Z and 0-9. Returns null if empty.</summary>
    private static string? NormalizeSerialPrefix(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = DocumentSerialPrefixHelper.NormalizeTurkishToAscii(value.Trim().ToUpperInvariant());
        var s = new string(normalized.Where(c => (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')).ToArray());
        if (s.Length == 0) return null;
        return s.Length > 3 ? s.Substring(0, 3) : s;
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
        LogoUrl = e.LogoUrl,
        InvoiceSerialPrefix = e.InvoiceSerialPrefix,
        DeliveryNoteSerialPrefix = e.DeliveryNoteSerialPrefix
    };
}
