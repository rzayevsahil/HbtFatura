namespace HbtFatura.Api.Entities;

public class LogEntry
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Level { get; set; } = "Info"; // Info, Warning, Error
    public string Message { get; set; } = string.Empty;
    public string? Action { get; set; } // CreateInvoice, Login, etc.
    public string? Module { get; set; } // Invoice, Order, Auth, etc.
    public Guid? UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; } // JSON or raw details
}
