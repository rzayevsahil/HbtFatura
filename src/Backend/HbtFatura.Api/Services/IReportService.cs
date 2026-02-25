using HbtFatura.Api.DTOs.Reports;

namespace HbtFatura.Api.Services;

public interface IReportService
{
    Task<CariExtractReportDto?> GetCariExtractAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<CashSummaryReportDto?> GetCashSummaryAsync(Guid? cashRegisterId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<BankSummaryReportDto?> GetBankSummaryAsync(Guid? bankAccountId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<StockLevelsReportDto> GetStockLevelsAsync(Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetCariExtractPdfAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<byte[]?> GetCariExtractExcelAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
}
