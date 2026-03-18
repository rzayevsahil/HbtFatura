using HbtFatura.Api.DTOs.Reports;

namespace HbtFatura.Api.Services;

public interface IReportService
{
    Task<CariExtractReportDto?> GetCariExtractAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<CashSummaryReportDto?> GetCashSummaryAsync(Guid? cashRegisterId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<BankSummaryReportDto?> GetBankSummaryAsync(Guid? bankAccountId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<StockLevelsReportDto> GetStockLevelsAsync(Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetStockLevelsPdfAsync(Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetCariExtractPdfAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<byte[]?> GetCariExtractExcelAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default);
    Task<InvoiceReportDto> GetInvoiceReportAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default);
    Task<byte[]?> GetInvoiceReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default);
    Task<byte[]?> GetInvoiceReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default);
    Task<MonthlyProductSalesReportDto> GetMonthlyProductSalesAsync(DateTime? dateFrom, DateTime? dateTo, Guid? productId, CancellationToken ct = default);
    Task<OrderReportDto> GetOrderReportAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetOrderReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetOrderReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<DeliveryNoteReportDto> GetDeliveryNoteReportAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetDeliveryNoteReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetDeliveryNoteReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default);
    Task<byte[]?> GetOrderDetailPdfAsync(Guid orderId, CancellationToken ct = default);
    Task<byte[]?> GetDeliveryNoteDetailPdfAsync(Guid deliveryNoteId, CancellationToken ct = default);
}
