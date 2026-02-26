using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HbtFatura.Api.DTOs.Reports;
using HbtFatura.Api.Services;

namespace HbtFatura.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("cari-extract")]
    public async Task<ActionResult<CariExtractReportDto>> GetCariExtract(
        [FromQuery] Guid customerId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string? format,
        CancellationToken ct = default)
    {
        if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
        {
            var pdf = await _service.GetCariExtractPdfAsync(customerId, dateFrom, dateTo, ct);
            if (pdf == null) return NotFound();
            return File(pdf, "application/pdf", $"cari-ekstre-{customerId:N}.pdf");
        }
        if (string.Equals(format, "xlsx", StringComparison.OrdinalIgnoreCase))
        {
            var excel = await _service.GetCariExtractExcelAsync(customerId, dateFrom, dateTo, ct);
            if (excel == null) return NotFound();
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"cari-ekstre-{customerId:N}.xlsx");
        }
        var data = await _service.GetCariExtractAsync(customerId, dateFrom, dateTo, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpGet("cash-summary")]
    public async Task<ActionResult<CashSummaryReportDto>> GetCashSummary(
        [FromQuery] Guid? cashRegisterId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken ct = default)
    {
        var data = await _service.GetCashSummaryAsync(cashRegisterId, dateFrom, dateTo, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpGet("bank-summary")]
    public async Task<ActionResult<BankSummaryReportDto>> GetBankSummary(
        [FromQuery] Guid? bankAccountId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        CancellationToken ct = default)
    {
        var data = await _service.GetBankSummaryAsync(bankAccountId, dateFrom, dateTo, ct);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpGet("stock-levels")]
    public async Task<ActionResult<StockLevelsReportDto>> GetStockLevels(
        [FromQuery] Guid? firmId,
        CancellationToken ct = default)
    {
        var data = await _service.GetStockLevelsAsync(firmId, ct);
        return Ok(data);
    }

    [HttpGet("invoice-report")]
    public async Task<ActionResult<InvoiceReportDto>> GetInvoiceReport(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] Guid? customerId,
        [FromQuery] string? format,
        CancellationToken ct = default)
    {
        if (string.Equals(format, "pdf", StringComparison.OrdinalIgnoreCase))
        {
            var pdf = await _service.GetInvoiceReportPdfAsync(dateFrom, dateTo, customerId, ct);
            if (pdf == null) return NotFound();
            return File(pdf, "application/pdf", "fatura-raporu.pdf");
        }
        if (string.Equals(format, "xlsx", StringComparison.OrdinalIgnoreCase))
        {
            var excel = await _service.GetInvoiceReportExcelAsync(dateFrom, dateTo, customerId, ct);
            if (excel == null) return NotFound();
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fatura-raporu.xlsx");
        }
        var data = await _service.GetInvoiceReportAsync(dateFrom, dateTo, customerId, ct);
        return Ok(data);
    }
}
