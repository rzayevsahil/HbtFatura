using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.AccountPayment;
using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class AccountPaymentService : IAccountPaymentService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogService _log;

    public AccountPaymentService(AppDbContext db, ICurrentUserContext currentUser, ILogService _log)
    {
        _db = db;
        _currentUser = currentUser;
        this._log = _log;
    }

    private IQueryable<Guid> AllowedCustomerIds(Guid? firmIdFilter)
    {
        var q = _db.Customers.IgnoreQueryFilters().Where(x => !x.IsDeleted).Select(c => new { c.Id, c.UserId, FirmId = c.User != null ? c.User.FirmId : (Guid?)null });
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return q.Where(x => x.FirmId == firmIdFilter.Value).Select(x => x.Id);
            return q.Select(x => x.Id);
        }
        if (_currentUser.FirmId.HasValue)
            return q.Where(x => x.FirmId == _currentUser.FirmId.Value).Select(x => x.Id);
        return q.Where(x => x.UserId == _currentUser.UserId).Select(x => x.Id);
    }

    public async Task<PagedResult<AccountPaymentListDto>> GetPagedAsync(int page, int pageSize, DateTime? dateFrom, DateTime? dateTo, Guid? customerId, string? type, Guid? firmId, CancellationToken ct = default)
    {
        var allowedIds = AllowedCustomerIds(firmId);
        var query = _db.AccountTransactions
            .Where(t => (t.ReferenceType == ReferenceType.Tahsilat || t.ReferenceType == ReferenceType.Odeme) && allowedIds.Contains(t.CustomerId));
        if (dateFrom.HasValue) query = query.Where(t => t.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(t => t.Date <= dateTo.Value.Date);
        if (customerId.HasValue) query = query.Where(t => t.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(type))
        {
            var t = type.Trim();
            if (string.Equals(t, "Tahsilat", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.ReferenceType == ReferenceType.Tahsilat);
            else if (string.Equals(t, "Odeme", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.ReferenceType == ReferenceType.Odeme);
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .Include(t => t.Customer)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new AccountPaymentListDto
            {
                Id = t.Id,
                Date = t.Date,
                Type = t.ReferenceType,
                CustomerId = t.CustomerId,
                CustomerTitle = t.Customer != null ? t.Customer.Title : "",
                Amount = t.Amount,
                Currency = t.Currency ?? "TRY",
                Description = t.Description ?? "",
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(ct);
        return new PagedResult<AccountPaymentListDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public async Task CreateAsync(AccountPaymentRequest request, CancellationToken ct = default)
    {
        var isTahsilat = string.Equals(request.Type, "Tahsilat", StringComparison.OrdinalIgnoreCase);
        var isOdeme = string.Equals(request.Type, "Odeme", StringComparison.OrdinalIgnoreCase);
        if (!isTahsilat && !isOdeme)
            throw new ArgumentException("Type must be Tahsilat or Odeme.");

        var customer = await _db.Customers.IgnoreQueryFilters()
            .Where(c => !c.IsDeleted && c.Id == request.CustomerId)
            .Where(c => _currentUser.IsSuperAdmin || (_currentUser.FirmId.HasValue && c.User != null && c.User.FirmId == _currentUser.FirmId.Value) || c.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(ct);
        if (customer == null)
            throw new ArgumentException("Customer not found or access denied.");

        var userId = customer.UserId;
        if (_currentUser.FirmId.HasValue)
            userId = _currentUser.UserId;

        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be positive.");

        if (string.Equals(request.PaymentMethod, "Kasa", StringComparison.OrdinalIgnoreCase))
        {
            if (!request.CashRegisterId.HasValue)
                throw new ArgumentException("CashRegisterId required for Kasa.");
            var hasAccess = await _db.CashRegisters.AnyAsync(x => x.Id == request.CashRegisterId && (x.FirmId == _currentUser.FirmId || _currentUser.IsSuperAdmin), ct);
            if (!hasAccess) throw new UnauthorizedAccessException("Cash register not found or access denied.");

            _db.CashTransactions.Add(new CashTransaction
            {
                Id = Guid.NewGuid(),
                CashRegisterId = request.CashRegisterId.Value,
                Date = request.Date,
                Type = isTahsilat ? CashTransactionType.Giris : CashTransactionType.Cikis,
                Amount = request.Amount,
                Description = request.Description?.Trim() ?? (isTahsilat ? "Cari tahsilat" : "Cari ödeme"),
                ReferenceType = isTahsilat ? ReferenceType.Tahsilat : ReferenceType.Odeme,
                CreatedAt = DateTime.UtcNow
            });
        }
        else if (string.Equals(request.PaymentMethod, "Banka", StringComparison.OrdinalIgnoreCase))
        {
            if (!request.BankAccountId.HasValue)
                throw new ArgumentException("BankAccountId required for Banka.");
            var hasAccess = await _db.BankAccounts.AnyAsync(x => x.Id == request.BankAccountId && (x.FirmId == _currentUser.FirmId || _currentUser.IsSuperAdmin), ct);
            if (!hasAccess) throw new UnauthorizedAccessException("Bank account not found or access denied.");

            _db.BankTransactions.Add(new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankAccountId = request.BankAccountId.Value,
                Date = request.Date,
                Type = isTahsilat ? BankTransactionType.Giris : BankTransactionType.Cikis,
                Amount = request.Amount,
                Description = request.Description?.Trim() ?? (isTahsilat ? "Cari tahsilat" : "Cari ödeme"),
                ReferenceType = isTahsilat ? ReferenceType.Tahsilat : ReferenceType.Odeme,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
            throw new ArgumentException("PaymentMethod must be Kasa or Banka.");

        // Cari ekstre ve diğer raporlarda ödeme kaynağını net görmek için açıklamaya ödeme yöntemini ekle.
        var baseDesc = request.Description?.Trim();
        var methodLabel = request.PaymentMethod == "Kasa" ? "Kasa" :
            request.PaymentMethod == "Banka" ? "Banka" : request.PaymentMethod;
        var defaultDesc = isTahsilat ? "Tahsilat" : "Ödeme";
        var descWithMethod = string.IsNullOrWhiteSpace(methodLabel)
            ? (baseDesc ?? defaultDesc)
            : $"{(baseDesc ?? defaultDesc)} ({methodLabel})";

        _db.AccountTransactions.Add(new AccountTransaction
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            UserId = userId,
            Date = request.Date,
            // Tahsilat = müşteri bize ödedi → Borç (bakiye düşer). Ödeme = biz müşteriye ödedik → Borç (bakiye düşer, cari ekstrede borç sütununda görünür; önceden Alacak yazılıyordu, yanlıştı).
            Type = AccountTransactionType.Borc,
            Amount = request.Amount,
            Currency = "TRY",
            Description = descWithMethod,
            ReferenceType = isTahsilat ? ReferenceType.Tahsilat : ReferenceType.Odeme,
            CreatedAt = DateTime.UtcNow
        });

        if (request.InvoiceId.HasValue && isTahsilat)
        {
            var invoice = await _db.Invoices
                .Where(i => i.Id == request.InvoiceId.Value && i.CustomerId == request.CustomerId && i.Status == InvoiceStatus.Issued)
                .FirstOrDefaultAsync(ct);
            if (invoice == null)
                throw new ArgumentException("Invoice not found, already paid/cancelled, or does not belong to this customer.");
            invoice.Status = InvoiceStatus.Paid;
        }

        await _db.SaveChangesAsync(ct);
        await _log.LogAsync($"{request.Type} işlemi yapıldı: {request.Amount:N2} TL", request.Type, "AccountPayment", "Info", $"Cari: {customer.Title}, Metod: {request.PaymentMethod}");
    }
}
