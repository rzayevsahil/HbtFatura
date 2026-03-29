namespace HbtFatura.Api.Services;

public sealed record InvoicePdfFile(byte[] Content, string InvoiceNumber);

public interface IInvoicePdfService
{
    Task<InvoicePdfFile?> GeneratePdfAsync(Guid invoiceId, CancellationToken ct = default);
}
