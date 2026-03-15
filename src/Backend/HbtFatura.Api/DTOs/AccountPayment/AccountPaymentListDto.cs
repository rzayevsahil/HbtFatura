namespace HbtFatura.Api.DTOs.AccountPayment;

public class AccountPaymentListDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty; // "Tahsilat" | "Odeme"
    public Guid CustomerId { get; set; }
    public string CustomerTitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
