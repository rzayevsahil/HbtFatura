using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Bank;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class BankAccountService : IBankAccountService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public BankAccountService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<BankAccount> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.BankAccounts.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.BankAccounts.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.BankAccounts.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.BankAccounts.Where(x => false);
    }

    public async Task<IReadOnlyList<BankAccountDto>> GetAllAsync(Guid? firmId, CancellationToken ct = default)
    {
        var list = await ScopeQuery(firmId)
            .OrderBy(x => x.Name)
            .Select(x => new BankAccountDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Name = x.Name,
                Iban = x.Iban,
                BankName = x.BankName,
                Currency = x.Currency,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                Balance = 0
            })
            .ToListAsync(ct);
        if (list.Count > 0)
        {
            var ids = list.Select(x => x.Id).ToList();
            var balances = await _db.BankTransactions
                .Where(t => ids.Contains(t.BankAccountId))
                .GroupBy(t => t.BankAccountId)
                .Select(g => new { Id = g.Key, Balance = g.Sum(t => t.Type == BankTransactionType.Giris ? t.Amount : -t.Amount) })
                .ToListAsync(ct);
            var dict = balances.ToDictionary(x => x.Id, x => x.Balance);
            foreach (var item in list)
                item.Balance = dict.GetValueOrDefault(item.Id, 0);
        }
        return list;
    }

    public async Task<BankAccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        var dto = new BankAccountDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Name = entity.Name,
            Iban = entity.Iban,
            BankName = entity.BankName,
            Currency = entity.Currency,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            Balance = await _db.BankTransactions.Where(t => t.BankAccountId == id).SumAsync(t => t.Type == BankTransactionType.Giris ? t.Amount : -t.Amount, ct)
        };
        return dto;
    }

    public async Task<BankAccountDto> CreateAsync(CreateBankAccountRequest request, CancellationToken ct = default)
    {
        Guid firmId;
        if (_currentUser.IsSuperAdmin && request.FirmId.HasValue)
            firmId = request.FirmId.Value;
        else if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        var entity = new BankAccount
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Name = request.Name.Trim(),
            Iban = request.Iban?.Trim(),
            BankName = request.BankName?.Trim(),
            Currency = request.Currency?.Trim() ?? "TRY",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.BankAccounts.Add(entity);
        await _db.SaveChangesAsync(ct);
        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<BankAccountDto?> UpdateAsync(Guid id, UpdateBankAccountRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        entity.Name = request.Name.Trim();
        entity.Iban = request.Iban?.Trim();
        entity.BankName = request.BankName?.Trim();
        entity.IsActive = request.IsActive;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        _db.BankAccounts.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PagedResult<BankTransactionDto>> GetTransactionsAsync(Guid bankAccountId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == bankAccountId, ct);
        if (!hasAccess)
            return new PagedResult<BankTransactionDto> { Items = new List<BankTransactionDto>(), TotalCount = 0, Page = page, PageSize = pageSize };

        var query = _db.BankTransactions.Where(t => t.BankAccountId == bankAccountId);
        if (dateFrom.HasValue) query = query.Where(t => t.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(t => t.Date <= dateTo.Value.Date);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(t => t.Date).ThenBy(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new BankTransactionDto
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
        return new PagedResult<BankTransactionDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<BankTransactionDto> AddTransactionAsync(Guid bankAccountId, CreateBankTransactionRequest request, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == bankAccountId, ct);
        if (!hasAccess) throw new UnauthorizedAccessException("Bank account not found or access denied.");

        var entity = new BankTransaction
        {
            Id = Guid.NewGuid(),
            BankAccountId = bankAccountId,
            Date = request.Date.Date,
            Type = request.Type,
            Amount = request.Amount,
            Description = request.Description?.Trim() ?? "",
            ReferenceType = ReferenceType.Manuel,
            CreatedAt = DateTime.UtcNow
        };
        _db.BankTransactions.Add(entity);
        await _db.SaveChangesAsync(ct);
        return new BankTransactionDto
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
