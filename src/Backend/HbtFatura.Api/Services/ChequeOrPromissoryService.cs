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

    public ChequeOrPromissoryService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<ChequeOrPromissory> ScopeQuery(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue) return _db.ChequeOrPromissories.Where(x => x.FirmId == firmIdFilter.Value);
            return _db.ChequeOrPromissories.AsQueryable();
        }
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.ChequeOrPromissories.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.ChequeOrPromissories.Where(x => false);
    }

    public async Task<PagedResult<ChequeOrPromissoryDto>> GetPagedAsync(int page, int pageSize, int? type, int? status, Guid? customerId, Guid? firmId, DateTime? dueFrom, DateTime? dueTo, CancellationToken ct = default)
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

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.DueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ChequeOrPromissoryDto
            {
                Id = x.Id,
                FirmId = x.FirmId,
                Type = x.Type,
                CustomerId = x.CustomerId,
                CustomerTitle = x.Customer.Title,
                Amount = x.Amount,
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
            CustomerId = entity.CustomerId,
            CustomerTitle = entity.Customer.Title,
            Amount = entity.Amount,
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
        else if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            firmId = _currentUser.FirmId.Value;
        else
            throw new UnauthorizedAccessException("Firm context required.");

        var hasCustomer = await _db.Customers.IgnoreQueryFilters().AnyAsync(c => c.Id == request.CustomerId && !c.IsDeleted && (c.User != null && c.User.FirmId == firmId), ct);
        if (!hasCustomer) throw new ArgumentException("Customer not found or access denied.");

        var entity = new ChequeOrPromissory
        {
            Id = Guid.NewGuid(),
            FirmId = firmId,
            Type = request.Type,
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
        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<ChequeOrPromissoryDto?> UpdateAsync(Guid id, UpdateChequeOrPromissoryRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        entity.Type = request.Type;
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
        entity.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        _db.ChequeOrPromissories.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
