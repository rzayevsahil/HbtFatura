namespace HbtFatura.Api.Entities;

public class GibSimulationSubmission
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid? SenderFirmId { get; set; }
    /// <summary>Alıcı firma; VKN eşleşmezse null (plan A: gönderim engellenir).</summary>
    public Guid? RecipientFirmId { get; set; }
    public string RecipientTaxNumber { get; set; } = string.Empty;
    public GibSimulationSubmissionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedByUserId { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public ApplicationUser SenderUser { get; set; } = null!;
    public Firm? SenderFirm { get; set; }
    public Firm? RecipientFirm { get; set; }
    public ApplicationUser? ResolvedByUser { get; set; }
}
