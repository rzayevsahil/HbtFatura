namespace HbtFatura.Api.DTOs.Reports;

public class CashSummaryReportDto
{
    public Guid? CashRegisterId { get; set; }
    public string? CashRegisterName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal TotalGiris { get; set; }
    public decimal TotalCikis { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<CashSummaryRowDto> Rows { get; set; } = new();
}

public class CashSummaryRowDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }
    public decimal Amount { get; set; }
}
