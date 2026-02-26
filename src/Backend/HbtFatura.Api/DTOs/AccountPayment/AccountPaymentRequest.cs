namespace HbtFatura.Api.DTOs.AccountPayment;

public class AccountPaymentRequest
{
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // "Kasa" | "Banka"
    public Guid? CashRegisterId { get; set; }
    public Guid? BankAccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Tahsilat" | "Odeme"
    /// <summary>When set (for Tahsilat), payment is linked to this invoice and invoice status is set to Paid.</summary>
    public Guid? InvoiceId { get; set; }
}
