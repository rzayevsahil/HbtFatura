using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public CustomerService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<Customer> ScopeQuery(Guid? firmIdFilter = null)
    {
        var baseQuery = _db.Customers.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return baseQuery.Where(x => x.User != null && x.User.FirmId == firmIdFilter.Value);
            return baseQuery;
        }
        if (_currentUser.IsFirmAdmin)
            return baseQuery.Where(x => x.User != null && x.User.FirmId == _currentUser.FirmId);
        return baseQuery.Where(x => x.UserId == _currentUser.UserId);
    }

    public async Task<PagedResult<CustomerListDto>> GetPagedAsync(int page, int pageSize, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var query = ScopeQuery(firmId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(s) ||
                (x.TaxNumber != null && x.TaxNumber.Contains(s)) ||
                (x.Email != null && x.Email.ToLower().Contains(s)));
        }
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new CustomerListDto
            {
                Id = x.Id,
                Title = x.Title,
                TaxNumber = x.TaxNumber,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
        return new PagedResult<CustomerListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<List<CustomerDto>> GetListForDropdownAsync(CancellationToken ct = default)
    {
        return await ScopeQuery()
            .OrderBy(x => x.Title)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                Title = x.Title,
                TaxNumber = x.TaxNumber,
                Address = x.Address,
                Phone = x.Phone,
                Email = x.Email
            })
            .ToListAsync(ct);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var entity = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            TaxNumber = request.TaxNumber?.Trim(),
            Address = request.Address?.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        _db.Customers.Add(entity);
        await _db.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        entity.Title = request.Title.Trim();
        entity.TaxNumber = request.TaxNumber?.Trim();
        entity.Address = request.Address?.Trim();
        entity.Phone = request.Phone?.Trim();
        entity.Email = request.Email?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;
        await _db.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return false;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static CustomerDto MapToDto(Customer e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        TaxNumber = e.TaxNumber,
        Address = e.Address,
        Phone = e.Phone,
        Email = e.Email
    };
}
