using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Cash;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class CashRegisterService : ICashRegisterService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public CashRegisterService(AppDbContext db, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _currentUser = currentUser;
        _log = log;
    }

    private IQueryable<CashRegister> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.CashRegisters.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.CashRegisters.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.CashRegisters.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.CashRegisters.Where(x => false);
    }

    public async Task<IReadOnlyList<CashRegisterDto>> GetAllAsync(Guid? firmId, CancellationToken ct = default)
    {
        var list = await ScopeQuery(firmId)
            .OrderBy(x => x.Name)
            .Select(x => new CashRegisterDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Name = x.Name,
                Currency = x.Currency,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                Balance = 0
            })
            .ToListAsync(ct);
        if (list.Count > 0)
        {
            var ids = list.Select(x => x.Id).ToList();
            var balances = await _db.CashTransactions
                .Where(t => ids.Contains(t.CashRegisterId))
                .GroupBy(t => t.CashRegisterId)
                .Select(g => new { Id = g.Key, Balance = g.Sum(t => t.Type == CashTransactionType.Giris ? t.Amount : -t.Amount) })
                .ToListAsync(ct);
            var dict = balances.ToDictionary(x => x.Id, x => x.Balance);
            foreach (var item in list)
                item.Balance = dict.GetValueOrDefault(item.Id, 0);
        }
        return list;
    }

    public async Task<CashRegisterDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        var dto = new CashRegisterDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Name = entity.Name,
            Currency = entity.Currency,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            Balance = await _db.CashTransactions.Where(t => t.CashRegisterId == id).SumAsync(t => t.Type == CashTransactionType.Giris ? t.Amount : -t.Amount, ct)
        };
        return dto;
    }

    public async Task<CashRegisterDto> CreateAsync(CreateCashRegisterRequest request, CancellationToken ct = default)
    {
        Guid firmId;
        if (_currentUser.IsSuperAdmin && request.FirmId.HasValue)
            firmId = request.FirmId.Value;
        else if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        var entity = new CashRegister
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Name = request.Name.Trim(),
            Currency = request.Currency?.Trim() ?? "TRY",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.CashRegisters.Add(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Kasa tanımlandı: {entity.Name}", "Create", "CashRegister", "Info", $"Id: {entity.Id}");
        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<CashRegisterDto?> UpdateAsync(Guid id, UpdateCashRegisterRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        entity.Name = request.Name.Trim();
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Kasa güncellendi: {entity.Name}", "Update", "CashRegister", "Info", $"Id: {entity.Id}");
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        _db.CashRegisters.Remove(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Kasa silindi: {entity.Name}", "Delete", "CashRegister", "Warning", $"Id: {entity.Id}");
        return true;
    }

    public async Task<PagedResult<CashTransactionDto>> GetTransactionsAsync(Guid cashRegisterId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == cashRegisterId, ct);
        if (!hasAccess)
            return new PagedResult<CashTransactionDto> { Items = new List<CashTransactionDto>(), TotalCount = 0, Page = page, PageSize = pageSize };

        var query = _db.CashTransactions.Where(t => t.CashRegisterId == cashRegisterId);
        if (dateFrom.HasValue) query = query.Where(t => t.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(t => t.Date <= dateTo.Value.Date);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(t => t.Date).ThenBy(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new CashTransactionDto
            {
                Id = t.Id,
                Date = t.Date,
                Type = t.Type,
                Amount = t.Amount,
                Description = t.Description,
                ReferenceType = t.ReferenceType,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(ct);
        return new PagedResult<CashTransactionDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<CashTransactionDto> AddTransactionAsync(Guid cashRegisterId, CreateCashTransactionRequest request, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == cashRegisterId, ct);
        if (!hasAccess) throw new UnauthorizedAccessException("Cash register not found or access denied.");

        var entity = new CashTransaction
        {
            Id = Guid.NewGuid(),
            CashRegisterId = cashRegisterId,
            Date = request.Date.Date,
            Type = request.Type,
            Amount = request.Amount,
            Description = request.Description?.Trim() ?? "",
            ReferenceType = ReferenceType.Manuel,
            CreatedAt = DateTime.UtcNow
        };
        _db.CashTransactions.Add(entity);
        await _db.SaveChangesAsync(ct);
        
        var reg = await _db.CashRegisters.FindAsync(cashRegisterId);
        await _log.LogAsync($"Kasaya manuel işlem eklendi: {request.Amount:N2} TL ({request.Type})", "AddTransaction", "CashRegister", "Info", $"Kasa: {reg?.Name}, Açıklama: {request.Description}");
        
        return new CashTransactionDto
        {
            Id = entity.Id,
            Date = entity.Date,
            Type = entity.Type,
            Amount = entity.Amount,
            Description = entity.Description,
            ReferenceType = entity.ReferenceType,
            CreatedAt = entity.CreatedAt
        };
    }
}
