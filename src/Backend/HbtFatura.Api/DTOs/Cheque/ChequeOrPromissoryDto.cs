namespace HbtFatura.Api.DTOs.Cheque;

public class ChequeOrPromissoryDto
{
    public Guid Id { get; set; }
    public Guid FirmId { get; set; }
    public int Type { get; set; }
    public string PortfolioNumber { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? BordroNumber { get; set; }
    public int? BordroType { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    /// <summary>Para birimi (tercihen bağlı banka hesabı; yoksa TRY).</summary>
    public string Currency { get; set; } = "TRY";
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public int Status { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid? BankAccountId { get; set; }
    public string? BankAccountName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateChequeOrPromissoryRequest
{
    public int Type { get; set; }
    public string? SerialNumber { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid? BankAccountId { get; set; }
    public Guid? FirmId { get; set; }
}

public class SetChequeStatusRequest
{
    public int Status { get; set; }
}

public class UpdateChequeOrPromissoryRequest
{
    public int Type { get; set; }
    public string? SerialNumber { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public Guid? BankAccountId { get; set; }
}
