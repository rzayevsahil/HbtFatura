namespace HbtFatura.Api.DTOs.Reports;

public class CariExtractReportDto
{
    public Guid CustomerId { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public List<CariExtractRowDto> Rows { get; set; } = new();
}

public class CariExtractRowDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Borc { get; set; }
    public decimal Alacak { get; set; }
    public decimal Bakiye { get; set; }
}
