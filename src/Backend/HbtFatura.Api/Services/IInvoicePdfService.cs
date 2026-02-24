namespace HbtFatura.Api.Services;

public interface IInvoicePdfService
{
    Task<byte[]?> GeneratePdfAsync(Guid invoiceId, CancellationToken ct = default);
}
