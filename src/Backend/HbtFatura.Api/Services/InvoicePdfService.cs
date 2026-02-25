using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;

namespace HbtFatura.Api.Services;

public class InvoicePdfService : IInvoicePdfService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public InvoicePdfService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private IQueryable<Invoice> ScopeQuery()
    {
        if (_currentUser.IsSuperAdmin)
            return _db.Invoices.AsQueryable();
        if (_currentUser.IsFirmAdmin)
            return _db.Invoices.Where(i => i.User != null && i.User.FirmId == _currentUser.FirmId);
        return _db.Invoices.Where(i => i.UserId == _currentUser.UserId);
    }

    public async Task<byte[]?> GeneratePdfAsync(Guid invoiceId, CancellationToken ct = default)
    {
        var invoice = await ScopeQuery()
            .Include(x => x.User)
            .Include(x => x.Items.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(x => x.Id == invoiceId, ct);
        if (invoice == null) return null;

        var firmId = invoice.User?.FirmId;
        CompanySettings? company = null;
        if (firmId.HasValue)
            company = await _db.CompanySettings.FirstOrDefaultAsync(x => x.FirmId == firmId.Value, ct);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(col =>
                {
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().Text(company?.CompanyName ?? "Firma").Bold().FontSize(14);
                            if (!string.IsNullOrEmpty(company?.TaxOffice)) c.Item().Text($"Vergi Dairesi: {company.TaxOffice}");
                            if (!string.IsNullOrEmpty(company?.TaxNumber)) c.Item().Text($"Vergi No: {company.TaxNumber}");
                            if (!string.IsNullOrEmpty(company?.Address)) c.Item().Text(company.Address);
                            if (!string.IsNullOrEmpty(company?.IBAN)) c.Item().Text($"IBAN: {company.IBAN}");
                        });
                        r.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("FATURA").Bold().FontSize(18);
                            c.Item().Text($"No: {invoice.InvoiceNumber}");
                            c.Item().Text($"Tarih: {invoice.InvoiceDate:dd.MM.yyyy}");
                        });
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().Text("Cari").Bold();
                    col.Item().Text(invoice.CustomerTitle);
                    if (!string.IsNullOrEmpty(invoice.CustomerTaxNumber)) col.Item().Text($"Vergi No/TC: {invoice.CustomerTaxNumber}");
                    if (!string.IsNullOrEmpty(invoice.CustomerAddress)) col.Item().Text(invoice.CustomerAddress);
                    col.Item().PaddingBottom(15);

                    col.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.RelativeColumn(3);
                            c.ConstantColumn(50);
                            c.ConstantColumn(60);
                            c.ConstantColumn(50);
                            c.ConstantColumn(70);
                            c.ConstantColumn(70);
                            c.ConstantColumn(70);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Element(CellStyle).Text("#");
                            h.Cell().Element(CellStyle).Text("Açıklama");
                            h.Cell().Element(CellStyle).AlignRight().Text("Miktar");
                            h.Cell().Element(CellStyle).AlignRight().Text("Birim Fiyat");
                            h.Cell().Element(CellStyle).AlignRight().Text("KDV %");
                            h.Cell().Element(CellStyle).AlignRight().Text("KDV Hariç");
                            h.Cell().Element(CellStyle).AlignRight().Text("KDV");
                            h.Cell().Element(CellStyle).AlignRight().Text("KDV Dahil");
                        });
                        var i = 1;
                        foreach (var item in invoice.Items)
                        {
                            t.Cell().Element(CellStyle).Text(i++.ToString());
                            t.Cell().Element(CellStyle).Text(item.Description);
                            t.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                            t.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("N2"));
                            t.Cell().Element(CellStyle).AlignRight().Text(item.VatRate.ToString("N0"));
                            t.Cell().Element(CellStyle).AlignRight().Text(item.LineTotalExclVat.ToString("N2"));
                            t.Cell().Element(CellStyle).AlignRight().Text(item.LineVatAmount.ToString("N2"));
                            t.Cell().Element(CellStyle).AlignRight().Text(item.LineTotalInclVat.ToString("N2"));
                        }
                    });

                    col.Item().AlignRight().PaddingTop(10).Column(c =>
                    {
                        c.Item().Row(r => { r.RelativeItem().Text("Ara Toplam:"); r.ConstantItem(80).AlignRight().Text(invoice.SubTotal.ToString("N2") + " " + invoice.Currency); });
                        c.Item().Row(r => { r.RelativeItem().Text("Toplam KDV:"); r.ConstantItem(80).AlignRight().Text(invoice.TotalVat.ToString("N2") + " " + invoice.Currency); });
                        c.Item().Row(r => { r.RelativeItem().Text("Genel Toplam:").Bold(); r.ConstantItem(80).AlignRight().Text(invoice.GrandTotal.ToString("N2") + " " + invoice.Currency).Bold(); });
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer c) => c.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);
}
