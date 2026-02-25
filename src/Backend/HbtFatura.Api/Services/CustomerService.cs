using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
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
                (x.Code != null && x.Code.ToLower().Contains(s)) ||
                (x.MainAccountCode != null && x.MainAccountCode.Contains(s)) ||
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
                MainAccountCode = x.MainAccountCode,
                Code = x.Code,
                AccountType = x.AccountType,
                Title = x.Title,
                TaxPayerType = x.TaxPayerType,
                CardType = x.CardType,
                TaxNumber = x.TaxNumber,
                Address = x.Address,
                City = x.City,
                District = x.District,
                PostalCode = x.PostalCode,
                Country = x.Country,
                Phone = x.Phone,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                Balance = 0
            })
            .ToListAsync(ct);
        if (items.Count > 0)
        {
            var ids = items.Select(x => x.Id).ToList();
            var balances = await _db.AccountTransactions
                .Where(t => ids.Contains(t.CustomerId))
                .GroupBy(t => t.CustomerId)
                .Select(g => new { CustomerId = g.Key, Balance = g.Sum(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount) })
                .ToListAsync(ct);
            var balanceDict = balances.ToDictionary(x => x.CustomerId, x => x.Balance);
            foreach (var item in items)
                item.Balance = balanceDict.GetValueOrDefault(item.Id, 0);
        }
        return new PagedResult<CustomerListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<List<CustomerDto>> GetListForDropdownAsync(CancellationToken ct = default)
    {
        return await ScopeQuery()
            .OrderBy(x => x.Title)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                MainAccountCode = x.MainAccountCode,
                Code = x.Code,
                AccountType = x.AccountType,
                Title = x.Title,
                TaxPayerType = x.TaxPayerType,
                CardType = x.CardType,
                TaxNumber = x.TaxNumber,
                Address = x.Address,
                City = x.City,
                District = x.District,
                PostalCode = x.PostalCode,
                Country = x.Country,
                Phone = x.Phone,
                Email = x.Email,
                Balance = 0
            })
            .ToListAsync(ct);
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await ScopeQuery().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return null;
        var dto = MapToDto(entity);
        dto.Balance = await GetBalanceAsync(id, ct);
        return dto;
    }

    public async Task<decimal> GetBalanceAsync(Guid customerId, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == customerId, ct);
        if (!hasAccess) return 0;
        var balance = await _db.AccountTransactions
            .Where(t => t.CustomerId == customerId)
            .SumAsync(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount, ct);
        return balance;
    }

    public async Task<PagedResult<AccountTransactionDto>> GetTransactionsAsync(Guid customerId, int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var hasAccess = await ScopeQuery().AnyAsync(x => x.Id == customerId, ct);
        if (!hasAccess)
            return new PagedResult<AccountTransactionDto> { Items = new List<AccountTransactionDto>(), TotalCount = 0, Page = page, PageSize = pageSize };

        var query = _db.AccountTransactions.Where(t => t.CustomerId == customerId);
        if (dateFrom.HasValue) query = query.Where(t => t.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(t => t.Date <= dateTo.Value.Date);

        var total = await query.CountAsync(ct);
        var orderedQuery = query.OrderBy(t => t.Date).ThenBy(t => t.CreatedAt);
        var list = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new { t.Id, t.Date, t.Description, t.Type, t.Amount, t.Currency, t.ReferenceType, t.ReferenceId, t.CreatedAt })
            .ToListAsync(ct);

        decimal openingBalance = 0;
        if (dateFrom.HasValue)
            openingBalance = await _db.AccountTransactions.Where(t => t.CustomerId == customerId && t.Date < dateFrom.Value.Date).SumAsync(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount, ct);
        else if (list.Count > 0)
        {
            var first = list[0];
            openingBalance = await _db.AccountTransactions
                .Where(t => t.CustomerId == customerId && (t.Date < first.Date || (t.Date == first.Date && t.CreatedAt < first.CreatedAt)))
                .SumAsync(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount, ct);
        }

        decimal running = openingBalance;
        var items = new List<AccountTransactionDto>();
        foreach (var t in list)
        {
            running += t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount;
            items.Add(new AccountTransactionDto
            {
                Id = t.Id,
                Date = t.Date,
                Description = t.Description,
                Type = t.Type,
                Amount = t.Amount,
                Currency = t.Currency,
                ReferenceType = t.ReferenceType,
                ReferenceId = t.ReferenceId,
                RunningBalance = running
            });
        }
        return new PagedResult<AccountTransactionDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var userId = _currentUser.UserId;
        var entity = new Customer
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MainAccountCode = request.MainAccountCode?.Trim(),
            Code = request.Code?.Trim(),
            AccountType = request.AccountType is Constants.AccountType.Tedarikci ? Constants.AccountType.Tedarikci : Constants.AccountType.Musteri,
            Title = request.Title.Trim(),
            TaxPayerType = request.TaxPayerType,
            CardType = request.CardType,
            TaxNumber = request.TaxNumber?.Trim(),
            Address = request.Address?.Trim(),
            City = request.City?.Trim(),
            District = request.District?.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            Country = request.Country?.Trim(),
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
        entity.MainAccountCode = request.MainAccountCode?.Trim();
        entity.Code = request.Code?.Trim();
        entity.AccountType = request.AccountType is Constants.AccountType.Tedarikci ? Constants.AccountType.Tedarikci : Constants.AccountType.Musteri;
        entity.Title = request.Title.Trim();
        entity.TaxPayerType = request.TaxPayerType;
        entity.CardType = request.CardType;
        entity.TaxNumber = request.TaxNumber?.Trim();
        entity.Address = request.Address?.Trim();
        entity.City = request.City?.Trim();
        entity.District = request.District?.Trim();
        entity.PostalCode = request.PostalCode?.Trim();
        entity.Country = request.Country?.Trim();
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
        MainAccountCode = e.MainAccountCode,
        Code = e.Code,
        AccountType = e.AccountType,
        Title = e.Title,
        TaxPayerType = e.TaxPayerType,
        CardType = e.CardType,
        TaxNumber = e.TaxNumber,
        Address = e.Address,
        City = e.City,
        District = e.District,
        PostalCode = e.PostalCode,
        Country = e.Country,
        Phone = e.Phone,
        Email = e.Email,
        Balance = 0
    };
}
