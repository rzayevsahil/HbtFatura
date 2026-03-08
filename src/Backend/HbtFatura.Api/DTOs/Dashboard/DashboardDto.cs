namespace HbtFatura.Api.DTOs.Dashboard;

public class DashboardDto
{
    public List<DashboardStatDto> Stats { get; set; } = new();
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    public List<RecentActivityDto> Activities { get; set; } = new();
}

public class DashboardStatDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Trend { get; set; }
}

public class RecentInvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class RecentActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}
