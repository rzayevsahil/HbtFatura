using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.AccountPayment;
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

    public async Task CreateAsync(AccountPaymentRequest request, CancellationToken ct = default)
    {
        var isTahsilat = string.Equals(request.Type, "Tahsilat", StringComparison.OrdinalIgnoreCase);
        var isOdeme = string.Equals(request.Type, "Odeme", StringComparison.OrdinalIgnoreCase);
        if (!isTahsilat && !isOdeme)
            throw new ArgumentException("Type must be Tahsilat or Odeme.");

        var customer = await _db.Customers.IgnoreQueryFilters()
            .Where(c => !c.IsDeleted && c.Id == request.CustomerId)
            .Where(c => _currentUser.IsSuperAdmin || (_currentUser.IsFirmAdmin && c.User != null && c.User.FirmId == _currentUser.FirmId) || c.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(ct);
        if (customer == null)
            throw new ArgumentException("Customer not found or access denied.");

        var userId = customer.UserId;
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
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
                Date = request.Date.Date,
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
                Date = request.Date.Date,
                Type = isTahsilat ? BankTransactionType.Giris : BankTransactionType.Cikis,
                Amount = request.Amount,
                Description = request.Description?.Trim() ?? (isTahsilat ? "Cari tahsilat" : "Cari ödeme"),
                ReferenceType = isTahsilat ? ReferenceType.Tahsilat : ReferenceType.Odeme,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
            throw new ArgumentException("PaymentMethod must be Kasa or Banka.");

        _db.AccountTransactions.Add(new AccountTransaction
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            UserId = userId,
            Date = request.Date.Date,
            Type = isTahsilat ? AccountTransactionType.Borc : AccountTransactionType.Alacak,
            Amount = request.Amount,
            Currency = "TRY",
            Description = request.Description?.Trim() ?? (isTahsilat ? "Tahsilat" : "Ödeme"),
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
