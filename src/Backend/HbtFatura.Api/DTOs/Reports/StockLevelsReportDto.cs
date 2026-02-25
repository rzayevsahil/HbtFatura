namespace HbtFatura.Api.DTOs.Reports;

public class StockLevelsReportDto
{
    public List<StockLevelRowDto> Items { get; set; } = new();
}

public class StockLevelRowDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal MinStock { get; set; }
    public decimal MaxStock { get; set; }
    public bool LowStock { get; set; }
}
