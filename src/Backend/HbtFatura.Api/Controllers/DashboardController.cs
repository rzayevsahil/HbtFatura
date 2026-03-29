using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Dashboard;
using HbtFatura.Api.Entities;
using System.Security.Claims;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetSummary(CancellationToken ct = default)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = Guid.Parse(userIdStr);
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        
        if (user == null) return Unauthorized();

        var isSuper = User.IsInRole(Roles.SuperAdmin);
        var firmId = user.FirmId;

        // Base queries
        var invoicesQuery = _db.Invoices.AsNoTracking();
        var ordersQuery = _db.Orders.AsNoTracking();
        var customersQuery = _db.Customers.AsNoTracking();
        var logsQuery = _db.LogEntries.AsNoTracking();

        if (!isSuper)
        {
            invoicesQuery = invoicesQuery.Where(i => i.User.FirmId == firmId);
            ordersQuery = ordersQuery.Where(o => o.User.FirmId == firmId);
            customersQuery = customersQuery.Where(c => c.User.FirmId == firmId);
            // Simple logic for logs: logs belonging to users of the same firm
            logsQuery = logsQuery.Where(l => _db.Users.Any(u => u.Id == l.UserId && u.FirmId == firmId));
        }

        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var response = new DashboardDto();

        if (isSuper)
        {
            // SuperAdmin Stats
            var totalFirms = await _db.Firms.CountAsync(ct);
            var totalUsers = await _db.Users.CountAsync(ct);
            var newFirmsThisMonth = await _db.Firms.CountAsync(f => f.CreatedAt >= startOfMonth, ct);
            var systemErrors = await _db.LogEntries.CountAsync(l => l.Level == "Error" && l.Timestamp >= startOfMonth, ct);

            response.Stats = new List<DashboardStatDto>
            {
                new() { Label = "Toplam Firma", Value = $"{totalFirms}", Icon = "business", Color = "teal", Trend = "Kayıtlı" },
                new() { Label = "Toplam Kullanıcı", Value = $"{totalUsers}", Icon = "people", Color = "blue", Trend = "Aktif" },
                new() { Label = "Yeni Firmalar", Value = $"{newFirmsThisMonth}", Icon = "add_business", Color = "orange", Trend = "Bu Ay" },
                new() { Label = "Sistem Hataları", Value = $"{systemErrors}", Icon = "report_problem", Color = "purple", Trend = "Kritik" }
            };

            // For SuperAdmin, RecentInvoices can show Recent Firms instead (reuse the list or change labels in UI)
            var firms = await _db.Firms
                .Include(f => f.CompanySettings)
                .OrderByDescending(f => f.CreatedAt)
                .Take(5)
                .ToListAsync(ct);

            response.RecentInvoices = firms.Select(f => new RecentInvoiceDto
            {
                Id = f.CompanySettings?.TaxNumber ?? "N/A",
                Customer = f.Name,
                Amount = "-", // Not applicable for firms
                Date = f.CreatedAt.ToString("dd.MM.yyyy"),
                Status = "Aktif",
                StatusCode = null
            }).ToList();

            var logsForSuper = await _db.LogEntries
                .OrderByDescending(l => l.Timestamp)
                .Take(5)
                .ToListAsync(ct);
            
            response.Activities = logsForSuper.Select(l => new RecentActivityDto
            {
                Id = l.Id.ToString(),
                Type = (l.Module ?? "system").ToLower(),
                Title = l.Action ?? "Sistem Olayı",
                Description = $"{l.UserFullName}: {l.Message}",
                Time = ""
            }).ToList();
        }
        else
        {
            // FirmAdmin / Employee Stats
            var monthlySales = await invoicesQuery
                .Where(i => i.InvoiceDate >= startOfMonth && i.InvoiceType == InvoiceType.Satis
                    && (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Paid))
                .SumAsync(i => i.GrandTotal, ct);
            
            var pendingPayments = await invoicesQuery
                .Where(i => i.Status == InvoiceStatus.Issued) 
                .SumAsync(i => i.GrandTotal, ct);

            var pendingCount = await invoicesQuery
                .Where(i => i.Status == InvoiceStatus.Issued)
                .CountAsync(ct);

            var newOrdersCount = await ordersQuery
                .Where(o => o.CreatedAt >= startOfMonth)
                .CountAsync(ct);

            var totalCustomers = await customersQuery.CountAsync(ct);

            response.Stats = new List<DashboardStatDto>
            {
                new() { Label = "Aylık Satış", Value = $"₺{monthlySales:N2}", Icon = "trending_up", Color = "teal", Trend = "Bu Ay" },
                new() { Label = "Bekleyen Tahsilat", Value = $"₺{pendingPayments:N2}", Icon = "payments", Color = "orange", Trend = $"{pendingCount} Fatura" },
                new() { Label = "Yeni Siparişler", Value = $"{newOrdersCount} Adet", Icon = "shopping_cart", Color = "blue", Trend = "Bekleyen" },
                new() { Label = "Toplam Müşteri", Value = $"{totalCustomers} Kayıt", Icon = "people", Color = "purple" }
            };

            var recentInvoices = await invoicesQuery
                .OrderByDescending(i => i.InvoiceDate)
                .Take(5)
                .ToListAsync(ct);

            var invoiceStatusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "InvoiceStatus", ct);

            response.RecentInvoices = recentInvoices.Select(i => new RecentInvoiceDto
            {
                Id = i.InvoiceNumber,
                Customer = i.CustomerTitle,
                Amount = $"₺{i.GrandTotal:N2}",
                Date = i.InvoiceDate.ToString("dd.MM.yyyy"),
                Status = LookupMaps.FormatIntCode((int)i.Status, invoiceStatusLabels),
                StatusCode = (int)i.Status
            }).ToList();

            var recentLogs = await logsQuery
                .OrderByDescending(l => l.Timestamp)
                .Take(5)
                .ToListAsync(ct);

            response.Activities = recentLogs.Select(l => new RecentActivityDto
            {
                Id = l.Id.ToString(),
                Type = (l.Module ?? "info").ToLower(),
                Title = l.Action ?? "İşlem",
                Description = l.Message,
                Time = ""
            }).ToList();
        }

        // Populate Time for both cases
        var finalLogsQuery = isSuper ? _db.LogEntries.AsNoTracking() : logsQuery;
        var logs = await finalLogsQuery.OrderByDescending(l => l.Timestamp).Take(5).ToListAsync(ct);
        for(int i=0; i < response.Activities.Count; i++) {
             // Matching by index since we took same top 5
             response.Activities[i].Time = GetTimeAgo(logs[i].Timestamp);
        }

        return Ok(response);
    }

    private static string GetTimeAgo(DateTime dateTime)
    {
        var span = DateTime.Now - dateTime;
        if (span.TotalMinutes < 1) return "Az önce";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} dak. önce";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours} saat önce";
        return $"{(int)span.TotalDays} gün önce";
    }
}
