using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public interface IInvoiceCalculationService
{
    void CalculateItemTotals(InvoiceItem item);
    void CalculateInvoiceTotals(Invoice invoice);
}
