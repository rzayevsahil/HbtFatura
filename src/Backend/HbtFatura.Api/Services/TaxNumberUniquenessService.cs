using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Validation;
using HbtFatura.Api.Helpers;

namespace HbtFatura.Api.Services;

public class TaxNumberUniquenessService : ITaxNumberUniquenessService
{
    private readonly AppDbContext _db;

    public TaxNumberUniquenessService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TaxNumberCheckResponseDto> CheckAsync(
        string? value,
        TaxNumberCheckMode mode,
        Guid? excludeCustomerId,
        Guid? excludeFirmIdForCompanyRow,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new TaxNumberCheckResponseDto
            {
                IsValidFormat = true,
                IsUnique = true,
                Normalized = null
            };
        }

        var norm = TaxNumberNormalization.Normalize(value);
        if (string.IsNullOrEmpty(norm))
        {
            return new TaxNumberCheckResponseDto
            {
                IsValidFormat = false,
                IsUnique = true,
                Message = "Geçersiz vergi no veya TC (10 veya 11 hane)."
            };
        }

        string? conflict;
        if (mode == TaxNumberCheckMode.Customer)
        {
            conflict = await GetConflictWithCompanyAsync(norm, excludeFirmId: null, ct);
            if (conflict == null)
                conflict = await GetConflictWithCustomerAsync(norm, excludeCustomerId, ct);
        }
        else
        {
            if (!excludeFirmIdForCompanyRow.HasValue)
            {
                return new TaxNumberCheckResponseDto
                {
                    IsValidFormat = true,
                    IsUnique = false,
                    Normalized = norm,
                    Message = "Firma bilgisi eksik."
                };
            }

            conflict = await GetConflictWithCompanyAsync(norm, excludeFirmIdForCompanyRow, ct);
            if (conflict == null)
                conflict = await GetConflictWithCustomerAsync(norm, excludeCustomerId: null, ct);
        }

        if (conflict != null)
        {
            return new TaxNumberCheckResponseDto
            {
                IsValidFormat = true,
                IsUnique = false,
                Normalized = norm,
                Message = conflict
            };
        }

        return new TaxNumberCheckResponseDto
        {
            IsValidFormat = true,
            IsUnique = true,
            Normalized = norm
        };
    }

    public async Task EnsureUniqueForCustomerAsync(string? taxNumber, Guid? excludeCustomerId, CancellationToken ct = default)
    {
        var norm = TaxNumberNormalization.Normalize(taxNumber);
        if (string.IsNullOrEmpty(norm))
            return;

        var conflict = await GetConflictWithCompanyAsync(norm, excludeFirmId: null, ct)
                       ?? await GetConflictWithCustomerAsync(norm, excludeCustomerId, ct);
        if (conflict != null)
            throw new InvalidOperationException(conflict);
    }

    public async Task EnsureUniqueForCompanyAsync(string? taxNumber, Guid firmId, CancellationToken ct = default)
    {
        var norm = TaxNumberNormalization.Normalize(taxNumber);
        if (string.IsNullOrEmpty(norm))
            return;

        var conflict = await GetConflictWithCompanyAsync(norm, firmId, ct)
                       ?? await GetConflictWithCustomerAsync(norm, excludeCustomerId: null, ct);
        if (conflict != null)
            throw new InvalidOperationException(conflict);
    }

    private async Task<string?> GetConflictWithCompanyAsync(string normalized, Guid? excludeFirmId, CancellationToken ct)
    {
        var rows = await _db.CompanySettings.AsNoTracking()
            .Where(x => x.TaxNumber != null && x.TaxNumber != "")
            .Select(x => new { x.FirmId, x.TaxNumber })
            .ToListAsync(ct);

        foreach (var x in rows)
        {
            if (excludeFirmId.HasValue && x.FirmId == excludeFirmId.Value)
                continue;
            if (TaxNumberNormalization.Normalize(x.TaxNumber) == normalized)
                return "Bu T.C. kimlik numarası veya vergi numarası başka bir firma şirket kaydında kullanılıyor.";
        }

        return null;
    }

    private async Task<string?> GetConflictWithCustomerAsync(string normalized, Guid? excludeCustomerId, CancellationToken ct)
    {
        var rows = await _db.Customers.AsNoTracking().IgnoreQueryFilters()
            .Where(c => !c.IsDeleted && c.TaxNumber != null && c.TaxNumber != "")
            .Select(c => new { c.Id, c.TaxNumber })
            .ToListAsync(ct);

        foreach (var x in rows)
        {
            if (excludeCustomerId.HasValue && x.Id == excludeCustomerId.Value)
                continue;
            if (TaxNumberNormalization.Normalize(x.TaxNumber) == normalized)
                return "Bu T.C. kimlik numarası veya vergi numarası başka bir cari kartta kullanılıyor.";
        }

        return null;
    }
}
