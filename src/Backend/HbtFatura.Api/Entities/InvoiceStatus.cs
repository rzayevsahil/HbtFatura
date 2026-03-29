namespace HbtFatura.Api.Entities;

public enum InvoiceStatus
{
    Draft = 0,
    Issued = 1,
    Paid = 2,
    Cancelled = 3,
    /// <summary>GİB simülasyonu: karşı taraf onayı bekleniyor.</summary>
    PendingGibAcceptance = 4
}
