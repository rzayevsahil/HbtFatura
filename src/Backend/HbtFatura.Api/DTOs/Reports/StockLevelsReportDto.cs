namespace HbtFatura.Api.DTOs.Reports;

public class StockLevelsReportDto
{
    public List<StockLevelRowDto> Items { get; set; } = new();
}

public class StockLevelRowDto
{
    public Guid ProductId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}
