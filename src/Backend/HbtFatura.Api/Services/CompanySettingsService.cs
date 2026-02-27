using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.CompanySettings;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class CompanySettingsService : ICompanySettingsService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public CompanySettingsService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<CompanySettingsDto?> GetByFirmIdAsync(Guid? firmId, CancellationToken ct = default)
    {
        var effectiveFirmId = firmId ?? _currentUser.FirmId;
        if (!effectiveFirmId.HasValue)
            return null;
        var entity = await _db.CompanySettings.FirstOrDefaultAsync(x => x.FirmId == effectiveFirmId.Value, ct);
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
        entity.TaxOffice = request.TaxOffice?.Trim();
        entity.TaxNumber = request.TaxNumber?.Trim();
        entity.Address = request.Address?.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Email = request.Email?.Trim();
        entity.IBAN = request.IBAN?.Trim();
        entity.BankName = request.BankName?.Trim();
        entity.LogoUrl = request.LogoUrl?.Trim();
        await _db.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    private static CompanySettingsDto MapToDto(CompanySettings e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        TaxOffice = e.TaxOffice,
        TaxNumber = e.TaxNumber,
        Address = e.Address,
        Phone = e.Phone,
        Email = e.Email,
        IBAN = e.IBAN,
        BankName = e.BankName,
        LogoUrl = e.LogoUrl
    };
}
