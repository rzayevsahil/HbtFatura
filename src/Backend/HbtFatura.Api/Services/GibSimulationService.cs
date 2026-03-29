using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.GibSimulation;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class GibSimulationService : IGibSimulationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IInvoiceService _invoiceService;
    private readonly IUserNotificationService _notifications;
    private readonly ILogService _log;

    public GibSimulationService(
        AppDbContext db,
        ICurrentUserContext currentUser,
        IInvoiceService invoiceService,
        IUserNotificationService notifications,
        ILogService log)
    {
        _db = db;
        _currentUser = currentUser;
        _invoiceService = invoiceService;
        _notifications = notifications;
        _log = log;
    }

    public async Task<IReadOnlyList<GibInboxItemDto>> GetInboxAsync(CancellationToken ct = default)
    {
        if (!_currentUser.FirmId.HasValue)
            return Array.Empty<GibInboxItemDto>();

        var firmId = _currentUser.FirmId.Value;
        return await _db.GibSimulationSubmissions.AsNoTracking()
            .Include(s => s.Invoice)
            .Include(s => s.SenderFirm)
            .Where(s => s.RecipientFirmId == firmId && s.Status == GibSimulationSubmissionStatus.Pending)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new GibInboxItemDto
            {
                SubmissionId = s.Id,
                InvoiceId = s.InvoiceId,
                InvoiceNumber = s.Invoice.InvoiceNumber,
                InvoiceDate = s.Invoice.InvoiceDate,
                SenderFirmName = s.SenderFirm != null ? s.SenderFirm.Name : "—",
                CustomerTitle = s.Invoice.CustomerTitle,
                RecipientTaxNumber = s.RecipientTaxNumber,
                GrandTotal = s.Invoice.GrandTotal,
                Currency = s.Invoice.Currency,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task AcceptAsync(Guid submissionId, CancellationToken ct = default)
    {
        if (!_currentUser.FirmId.HasValue)
            throw new InvalidOperationException("Firma bağlamı gerekli.");

        var firmId = _currentUser.FirmId.Value;
        var submission = await _db.GibSimulationSubmissions
            .Include(s => s.Invoice)
                .ThenInclude(i => i.Items)
            .Include(s => s.SenderFirm)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.RecipientFirmId == firmId, ct);

        if (submission == null)
            throw new InvalidOperationException("Kayıt bulunamadı.");
        if (submission.Status != GibSimulationSubmissionStatus.Pending)
            throw new InvalidOperationException("Bu kayıt zaten sonuçlandırılmış.");

        var invoice = submission.Invoice;
        if (invoice.InvoiceType != InvoiceType.Satis)
            throw new InvalidOperationException("Geçersiz fatura tipi.");

        await _invoiceService.ApplyIssuedForGibAcceptedSaleAsync(invoice, _currentUser.UserId, ct);

        submission.Status = GibSimulationSubmissionStatus.Accepted;
        submission.ResolvedAt = DateTime.UtcNow;
        submission.ResolvedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        if (submission.SenderFirmId.HasValue)
        {
            await _notifications.NotifyUsersInFirmAsync(
                submission.SenderFirmId.Value,
                NotificationTypes.GibInvoiceAccepted,
                "GİB simülasyonu: fatura onaylandı",
                $"{invoice.InvoiceNumber} numaralı fatura karşı tarafça onaylandı (simülasyon).",
                ReferenceType.Fatura,
                invoice.Id,
                ct);
        }

        await _log.LogAsync($"GİB simülasyonu onayı: {invoice.InvoiceNumber}", "GibAccept", "GibSimulation", "Info", $"SubmissionId: {submissionId}, InvoiceId: {invoice.Id}");
    }

    public async Task RejectAsync(Guid submissionId, CancellationToken ct = default)
    {
        if (!_currentUser.FirmId.HasValue)
            throw new InvalidOperationException("Firma bağlamı gerekli.");

        var firmId = _currentUser.FirmId.Value;
        var submission = await _db.GibSimulationSubmissions
            .Include(s => s.Invoice)
            .Include(s => s.SenderFirm)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.RecipientFirmId == firmId, ct);

        if (submission == null)
            throw new InvalidOperationException("Kayıt bulunamadı.");
        if (submission.Status != GibSimulationSubmissionStatus.Pending)
            throw new InvalidOperationException("Bu kayıt zaten sonuçlandırılmış.");

        var invoice = submission.Invoice;
        submission.Status = GibSimulationSubmissionStatus.Rejected;
        submission.ResolvedAt = DateTime.UtcNow;
        submission.ResolvedByUserId = _currentUser.UserId;

        invoice.Status = InvoiceStatus.Draft;
        invoice.UpdatedAt = DateTime.UtcNow;
        invoice.UpdatedBy = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        if (submission.SenderFirmId.HasValue)
        {
            await _notifications.NotifyUsersInFirmAsync(
                submission.SenderFirmId.Value,
                NotificationTypes.GibInvoiceRejected,
                "GİB simülasyonu: fatura reddedildi",
                $"{invoice.InvoiceNumber} numaralı fatura karşı tarafça reddedildi; fatura taslak olarak geri alındı (simülasyon).",
                ReferenceType.Fatura,
                invoice.Id,
                ct);
        }

        await _log.LogAsync($"GİB simülasyonu ret: {invoice.InvoiceNumber}", "GibReject", "GibSimulation", "Info", $"SubmissionId: {submissionId}");
    }
}
