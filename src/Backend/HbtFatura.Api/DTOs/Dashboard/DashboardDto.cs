namespace HbtFatura.Api.DTOs.Dashboard;

public class DashboardDto
{
    public List<DashboardStatDto> Stats { get; set; } = new();
    public List<RecentInvoiceDto> RecentInvoices { get; set; } = new();
    public List<RecentActivityDto> Activities { get; set; } = new();
}

public class DashboardStatDto
{
    /// <summary>İstemci çevirisi için sabit anahtar (örn. monthly_sales).</summary>
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Trend { get; set; }
    /// <summary>TRY tutarı (aylık satış, bekleyen tahsilat).</summary>
    public decimal? Amount { get; set; }
    /// <summary>Sayı göstergeleri (sipariş adedi, müşteri sayısı, firma sayısı vb.).</summary>
    public int? Count { get; set; }
    /// <summary>Trend metni için ek sayı (örn. bekleyen fatura adedi).</summary>
    public int? TrendCount { get; set; }
    /// <summary>Trend çevirisi: this_month, invoice_count, pending, registered, active, critical.</summary>
    public string? TrendKind { get; set; }
}

public class RecentInvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    /// <summary>Fatura durumu enum değeri (firma özeti); SuperAdmin firma satırında null.</summary>
    public int? StatusCode { get; set; }
}

public class RecentActivityDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}
