using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class InvoiceCalculationService : IInvoiceCalculationService
{
    public void CalculateItemTotals(InvoiceItem item)
    {
        var discountMultiplier = 1m - (item.DiscountPercent / 100m);
        item.LineTotalExclVat = Math.Round(item.Quantity * item.UnitPrice * discountMultiplier, 2);
        item.LineVatAmount = Math.Round(item.LineTotalExclVat * (item.VatRate / 100m), 2);
        item.LineTotalInclVat = item.LineTotalExclVat + item.LineVatAmount;
    }

    public void CalculateInvoiceTotals(Invoice invoice)
    {
        invoice.SubTotal = invoice.Items.Sum(x => x.LineTotalExclVat);
        invoice.TotalVat = invoice.Items.Sum(x => x.LineVatAmount);
        invoice.GrandTotal = invoice.Items.Sum(x => x.LineTotalInclVat);
        invoice.SubTotal = Math.Round(invoice.SubTotal, 2);
        invoice.TotalVat = Math.Round(invoice.TotalVat, 2);
        invoice.GrandTotal = Math.Round(invoice.GrandTotal, 2);
    }
}
