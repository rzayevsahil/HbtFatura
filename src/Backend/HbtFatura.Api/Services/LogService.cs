using Microsoft.EntityFrameworkCore;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;
using HbtFatura.Api.DTOs.Customers;

namespace HbtFatura.Api.Services;

public class LogService : ILogService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogService(AppDbContext db, ICurrentUserContext currentUser, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _currentUser = currentUser;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string message, string? action = null, string? module = null, string level = "Info", string? details = null)
    {
        try
        {
            var log = new LogEntry
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                Action = action,
                Module = module,
                Details = details,
                UserId = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null,
                UserFullName = _currentUser.IsAuthenticated ? _currentUser.FullName : "System/Anonymous",
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            };

            _db.LogEntries.Add(log);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Loglama hatası ana akışı bozmamalı
            System.Diagnostics.Debug.WriteLine($"Loglama Hatası: {ex.Message}");
        }
    }

    public async Task<PagedResult<LogEntry>> GetLogsAsync(int page, int pageSize, string? level = null, string? module = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var query = _db.LogEntries.AsNoTracking();

        if (!string.IsNullOrEmpty(level)) query = query.Where(x => x.Level == level);
        if (!string.IsNullOrEmpty(module)) query = query.Where(x => x.Module == module);
        if (dateFrom.HasValue) query = query.Where(x => x.Timestamp >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(x => x.Timestamp <= dateTo.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<LogEntry> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }
}
