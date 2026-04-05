using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Cheque;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class ChequeOrPromissoryService : IChequeOrPromissoryService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public ChequeOrPromissoryService(AppDbContext db, ICurrentUserContext currentUser, ILogService log)
    {
        _db = db;
        _currentUser = currentUser;
        _log = log;
    }

    private IQueryable<ChequeOrPromissory> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue) return _db.ChequeOrPromissories.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.ChequeOrPromissories.AsQueryable();
        }
        if (_currentUser.FirmId.HasValue)
            return _db.ChequeOrPromissories.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.ChequeOrPromissories.Where(x => false);
    }

    public async Task<PagedResult<ChequeOrPromissoryDto>> GetPagedAsync(int page, int pageSize, int? type, int? status, Guid? customerId, Guid? firmId, DateTime? dueFrom, DateTime? dueTo, string? portfolioNumber = null, string? serialNumber = null, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId)
            .Include(x => x.Customer)
            .Include(x => x.BankAccount)
            .AsQueryable();
        if (type.HasValue) query = query.Where(x => x.Type == type.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (dueFrom.HasValue) query = query.Where(x => x.DueDate >= dueFrom.Value.Date);
        if (dueTo.HasValue) query = query.Where(x => x.DueDate <= dueTo.Value.Date);
        if (!string.IsNullOrWhiteSpace(portfolioNumber))
            query = query.Where(x => x.PortfolioNumber != null && x.PortfolioNumber.Contains(portfolioNumber.Trim()));
        if (!string.IsNullOrWhiteSpace(serialNumber))
            query = query.Where(x => x.SerialNumber != null && x.SerialNumber.Contains(serialNumber.Trim()));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.PortfolioNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ChequeOrPromissoryDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Type = x.Type,
                PortfolioNumber = x.PortfolioNumber,
                SerialNumber = x.SerialNumber,
                BordroNumber = x.BordroNumber,
                BordroType = x.BordroType,
                CustomerId = x.CustomerId,
                CustomerTitle = x.Customer.Title,
                Amount = x.Amount,
                Currency = x.BankAccount != null && !string.IsNullOrWhiteSpace(x.BankAccount.Currency)
                    ? x.BankAccount.Currency.Trim().ToUpperInvariant()
                    : "TRY",
                IssueDate = x.IssueDate,
                DueDate = x.DueDate,
                Status = x.Status,
                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,
                BankAccountId = x.BankAccountId,
                BankAccountName = x.BankAccount != null ? x.BankAccount.Name : null,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
        return new PagedResult<ChequeOrPromissoryDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<ChequeOrPromissoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery()
            .Include(x => x.Customer)
            .Include(x => x.BankAccount)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        return new ChequeOrPromissoryDto
        {
            Id = entity.Id,
            FirmId = entity.FirmId,
            Type = entity.Type,
            PortfolioNumber = entity.PortfolioNumber,
            SerialNumber = entity.SerialNumber,
            BordroNumber = entity.BordroNumber,
            BordroType = entity.BordroType,
            CustomerId = entity.CustomerId,
            CustomerTitle = entity.Customer.Title,
            Amount = entity.Amount,
            Currency = entity.BankAccount != null && !string.IsNullOrWhiteSpace(entity.BankAccount.Currency)
                ? entity.BankAccount.Currency.Trim().ToUpperInvariant()
                : "TRY",
            IssueDate = entity.IssueDate,
            DueDate = entity.DueDate,
            Status = entity.Status,
            ReferenceType = entity.ReferenceType,
            ReferenceId = entity.ReferenceId,
            BankAccountId = entity.BankAccountId,
            BankAccountName = entity.BankAccount?.Name,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<ChequeOrPromissoryDto> CreateAsync(CreateChequeOrPromissoryRequest request, CancellationToken ct = default)
    {
        Guid firmId;
        if (_currentUser.IsSuperAdmin && request.FirmId.HasValue)
            firmId = request.FirmId.Value;
        else if (_currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        var hasCustomer = await _db.Customers.IgnoreQueryFilters().AnyAsync(c => c.Id == request.CustomerId && !c.IsDeleted && (c.User != null && c.User.FirmId == firmId), ct);
        if (!hasCustomer) throw new ArgumentException("Customer not found or access denied.");

        var year = request.IssueDate.Year;
        var portfolioNumber = await GetNextPortfolioNumberAsync(firmId, request.Type, year, ct);
        var bordroType = request.Type == ChequeType.Cek ? BordroType.CekGirisi : BordroType.SenetGirisi;
        var bordroNumber = await GetNextBordroNumberAsync(firmId, bordroType, year, ct);

        var entity = new ChequeOrPromissory
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Type = request.Type,
            PortfolioNumber = portfolioNumber,
            SerialNumber = string.IsNullOrWhiteSpace(request.SerialNumber) ? null : request.SerialNumber.Trim(),
            BordroNumber = bordroNumber,
            BordroType = bordroType,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            IssueDate = request.IssueDate.Date,
            DueDate = request.DueDate.Date,
            Status = ChequeStatus.Portföyde,
            ReferenceType = request.ReferenceType,
            ReferenceId = request.ReferenceId,
            BankAccountId = request.BankAccountId,
            CreatedAt = DateTime.UtcNow
        };
        _db.ChequeOrPromissories.Add(entity);
        await _db.SaveChangesAsync(ct);
        var typeLabel = request.Type == ChequeType.Cek ? "Çek" : "Senet";
        await _log.LogAsync($"{typeLabel} portföye eklendi: {entity.PortfolioNumber}", "Create", "ChequeOrPromissory", "Info", $"Id: {entity.Id}, FirmId: {firmId}, Tutar: {entity.Amount}");
        return (await GetByIdAsync(entity.Id, ct))!;
    }

    private async Task<string> GetNextPortfolioNumberAsync(Guid firmId, int type, int year, CancellationToken ct)
    {
        var prefix = type == ChequeType.Cek ? "CEK" : "SNT";
        var yearStr = year.ToString();
        var pattern = prefix + yearStr;
        var last = await _db.ChequeOrPromissories
            .Where(x => x.FirmId == firmId && x.PortfolioNumber.StartsWith(pattern))
            .OrderByDescending(x => x.PortfolioNumber)
            .Select(x => x.PortfolioNumber)
            .FirstOrDefaultAsync(ct);
        if (string.IsNullOrEmpty(last) || last.Length < pattern.Length + 9)
            return $"{pattern}-000000001";
        var seqStr = last.Length > pattern.Length + 1 ? last.Substring(pattern.Length + 1) : "";
        if (long.TryParse(seqStr, out var num))
            return $"{pattern}-{(num + 1):D9}";
        return $"{pattern}-000000001";
    }

    private async Task<string> GetNextBordroNumberAsync(Guid firmId, int bordroType, int year, CancellationToken ct)
    {
        var prefix = bordroType == BordroType.CekGirisi ? "CG" : "SG";
        var yearStr = year.ToString();
        var pattern = prefix + yearStr;
        var last = await _db.ChequeOrPromissories
            .Where(x => x.FirmId == firmId && x.BordroType == bordroType && x.BordroNumber != null && x.BordroNumber.StartsWith(pattern))
            .OrderByDescending(x => x.BordroNumber)
            .Select(x => x.BordroNumber)
            .FirstOrDefaultAsync(ct);
        if (string.IsNullOrEmpty(last) || last.Length < pattern.Length + 9)
            return $"{pattern}-000000001";
        var seqStr = last.Length > pattern.Length + 1 ? last.Substring(pattern.Length + 1) : "";
        if (long.TryParse(seqStr, out var num))
            return $"{pattern}-{(num + 1):D9}";
        return $"{pattern}-000000001";
    }

    public async Task<ChequeOrPromissoryDto?> UpdateAsync(Guid id, UpdateChequeOrPromissoryRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        entity.Type = request.Type;
        entity.SerialNumber = string.IsNullOrWhiteSpace(request.SerialNumber) ? null : request.SerialNumber.Trim();
        entity.CustomerId = request.CustomerId;
        entity.Amount = request.Amount;
        entity.IssueDate = request.IssueDate.Date;
        entity.DueDate = request.DueDate.Date;
        entity.ReferenceType = request.ReferenceType;
        entity.ReferenceId = request.ReferenceId;
        entity.BankAccountId = request.BankAccountId;
        await _db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> SetStatusAsync(Guid id, int status, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        var portfolio = entity.PortfolioNumber ?? id.ToString();
        entity.Status = status;
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Çek/senet durumu güncellendi: {portfolio} → {status}", "SetStatus", "ChequeOrPromissory", "Info", $"Id: {id}");
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        var portfolio = entity.PortfolioNumber ?? id.ToString();
        var firmId = entity.FirmId;
        _db.ChequeOrPromissories.Remove(entity);
        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"Çek/senet silindi: {portfolio}", "Delete", "ChequeOrPromissory", "Warning", $"Id: {id}, FirmId: {firmId}");
        return true;
    }
}
