using HbtFatura.Api.DTOs.Customers;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public interface ILogService
{
    Task LogAsync(string message, string? action = null, string? module = null, string level = "Info", string? details = null);
    Task<PagedResult<LogEntry>> GetLogsAsync(int page, int pageSize, string? level = null, string? module = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
}
