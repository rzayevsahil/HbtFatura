using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.MainAccountCode;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class MainAccountCodeService : IMainAccountCodeService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public MainAccountCodeService(AppDbContext db, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _currentUser = currentUser;
        _log = log;
    }

    /// <summary>Firma kapsamındaki kodlar (sistem kodları hariç). Update/Delete sadece bu kapsamdakilerde yapılabilir.</summary>
    private IQueryable<MainAccountCode> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.MainAccountCodes.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.MainAccountCodes.Where(x => x.FirmId != null);
        }
        if (_currentUser.FirmId.HasValue)
            return _db.MainAccountCodes.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.MainAccountCodes.Where(x => false);
    }

    private Guid? EffectiveFirmIdForList(Guid? firmIdFilter)
    {
        if (firmIdFilter.HasValue && _currentUser.IsSuperAdmin)
            return firmIdFilter.Value;
        return _currentUser.FirmId;
    }

    private static Guid ResolveFirmId(CreateMainAccountCodeRequest request, ICurrentUserContext currentUser)
    {
        if (request.FirmId.HasValue && currentUser.IsSuperAdmin)
            return request.FirmId.Value;
        if (currentUser.FirmId.HasValue)
            return currentUser.FirmId.Value;
        throw new UnauthorizedAccessException("Firma bilgisi gerekli.");
    }

    /// <summary>Firma + sistem kodlarını döner. Liste Code alanına göre sıralıdır.</summary>
    public async Task<IReadOnlyList<MainAccountCodeDto>> GetByFirmAsync(Guid? firmId, CancellationToken ct = default)
    {
        var effectiveFirmId = EffectiveFirmIdForList(firmId);
        var query = _db.MainAccountCodes
            .Where(x => x.FirmId == null || x.FirmId == effectiveFirmId)
            .OrderBy(x => x.Code)
            .Select(x => new MainAccountCodeDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Code = x.Code,
                Name = x.Name,
                SortOrder = x.SortOrder,
                CreatedAt = x.CreatedAt,
                IsSystem = x.FirmId == null
            });
        return await query.ToListAsync(ct);
    }

    public async Task<MainAccountCodeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _db.MainAccountCodes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        if (entity.FirmId != null && !await ScopeQuery().AnyAsync(x => x.Id == id, ct))
            return null;
        return new MainAccountCodeDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Code = entity.Code,
            Name = entity.Name,
            SortOrder = entity.SortOrder,
            CreatedAt = entity.CreatedAt,
            IsSystem = entity.FirmId == null
        };
    }

    public async Task<MainAccountCodeDto> CreateAsync(CreateMainAccountCodeRequest request, CancellationToken ct = default)
    {
        var firmId = ResolveFirmId(request, _currentUser);
        var code = request.Code.Trim();
        var exists = await _db.MainAccountCodes.AnyAsync(x => x.FirmId == firmId && x.Code == code, ct);
        if (exists)
            throw new InvalidOperationException("Bu hesap kodu zaten kayıtlı. Farklı bir kod girin.");

        var entity = new MainAccountCode
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Code = code,
            Name = request.Name.Trim(),
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow
        };
        _db.MainAccountCodes.Add(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ana hesap kodu oluşturuldu: {entity.Code} — {entity.Name}", "Create", "MainAccountCode", "Info", $"Id: {entity.Id}, FirmId: {firmId}");
        return new MainAccountCodeDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Code = entity.Code,
            Name = entity.Name,
            SortOrder = entity.SortOrder,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<MainAccountCodeDto?> UpdateAsync(Guid id, UpdateMainAccountCodeRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        if (entity.FirmId == null)
            throw new InvalidOperationException("Sistem kodları düzenlenemez.");

        var code = request.Code.Trim();
        var exists = await _db.MainAccountCodes.AnyAsync(x => x.FirmId == entity.FirmId && x.Code == code && x.Id != id, ct);
        if (exists)
            throw new InvalidOperationException("Bu hesap kodu zaten kayıtlı. Farklı bir kod girin.");

        entity.Code = code;
        entity.Name = request.Name.Trim();
        entity.SortOrder = request.SortOrder;
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ana hesap kodu güncellendi: {entity.Code} — {entity.Name}", "Update", "MainAccountCode", "Info", $"Id: {entity.Id}");
        return new MainAccountCodeDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Code = entity.Code,
            Name = entity.Name,
            SortOrder = entity.SortOrder,
            CreatedAt = entity.CreatedAt,
            IsSystem = false
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        if (entity.FirmId == null)
            throw new InvalidOperationException("Sistem kodları silinemez.");
        var code = entity.Code;
        var name = entity.Name;
        var firmId = entity.FirmId;
        _db.MainAccountCodes.Remove(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Ana hesap kodu silindi: {code} — {name}", "Delete", "MainAccountCode", "Warning", $"Id: {id}, FirmId: {firmId}");
        return true;
    }
}
