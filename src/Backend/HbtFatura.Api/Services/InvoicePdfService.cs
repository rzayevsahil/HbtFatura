using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;
using QRCoder;

namespace HbtFatura.Api.Services;

public class InvoicePdfService : IInvoicePdfService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

    static InvoicePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public InvoicePdfService(AppDbContext db, ICurrentUserContext currentUser, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
    {
        _db = db;
        _currentUser = currentUser;
        _env = env;
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
                .ThenInclude(i => i.Product)
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
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Verdana));

                // --- HEADER SECTION ---
                page.Header().Column(col =>
                {
                    // 0. Top Line (New request)
                    col.Item().Row(row => 
                    {
                        row.RelativeItem(4).LineHorizontal(2.0f).LineColor(Colors.Black);
                        row.RelativeItem(6);
                    });

                    col.Item().PaddingTop(5).Row(r =>
                    {
                        // 1. Company Info (Left)
                        r.RelativeItem(1).Column(c =>
                        {
                            c.Item().Text(company?.CompanyName ?? "Firma Adı").FontSize(8);
                            c.Item().PaddingTop(4).Text(company?.Address ?? "").FontSize(8);
                            if (!string.IsNullOrEmpty(company?.Phone)) c.Item().Text($"Tel: {company.Phone}").FontSize(8);
                            if (!string.IsNullOrEmpty(company?.Email)) c.Item().Text($"E-Posta: {company.Email}").FontSize(8);
                            if (!string.IsNullOrEmpty(company?.TaxOffice)) c.Item().Text($"Vergi Dairesi: {company.TaxOffice}").FontSize(8);
                            if (!string.IsNullOrEmpty(company?.TaxNumber)) c.Item().Text($"VKN/TCKN: {company.TaxNumber}").FontSize(8);
                        });

                        // 2. Logo & e-FATURA (Center)
                        r.RelativeItem(1).Column(c =>
                        {
                            string? gibLogoPath = null;
                            var logoFileName = "logos/giblogo.png";
                            
                            // Try WebRootPath first
                            if (!string.IsNullOrEmpty(_env.WebRootPath))
                            {
                                var path = System.IO.Path.Combine(_env.WebRootPath, logoFileName);
                                if (System.IO.File.Exists(path)) gibLogoPath = path;
                            }
                            
                            // Fallback to ContentRootPath/wwwroot
                            if (gibLogoPath == null)
                            {
                                var path = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot", logoFileName);
                                if (System.IO.File.Exists(path)) gibLogoPath = path;
                            }

                            if (!string.IsNullOrEmpty(gibLogoPath))
                            {
                                // Remove fixed width to allow AlignCenter to work on the actual image width
                                c.Item().AlignCenter().Height(80).Image(gibLogoPath, ImageScaling.FitHeight);
                            }
                            else
                            {
                                c.Item().AlignCenter().Height(40).Text("GİB LOGO").FontSize(10).Italic();
                            }
                            c.Item().AlignCenter().Text("e-FATURA").Bold().FontSize(11);
                        });

                        r.RelativeItem(1).Column(c =>
                        {
                            c.Item().AlignRight().Width(80).Image(GenerateQrCode(invoice.InvoiceNumber));
                        });
                    });

                    // Line shortened to left side (40%) - Increased thickness to 2.0f
                    col.Item().PaddingTop(10).Row(row => 
                    {
                        row.RelativeItem(4).LineHorizontal(2.0f).LineColor(Colors.Black);
                        row.RelativeItem(6);
                    });
                });

                // --- CONTENT SECTION ---
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Line above SAYIN (New request)
                    col.Item().Row(row => 
                    {
                        row.RelativeItem(4).LineHorizontal(2.0f).LineColor(Colors.Black);
                        row.RelativeItem(6);
                    });

                    // Sayın (Customer Info) + Metadata Table
                    col.Item().PaddingTop(5).Row(r =>
                    {
                        r.RelativeItem().Column(c =>
                        {
                            c.Item().Text("SAYIN").Bold().FontSize(10);
                            c.Item().Text(invoice.CustomerTitle).FontSize(8);
                            c.Item().PaddingTop(4).Text(invoice.CustomerAddress ?? "").FontSize(8);
                            if (!string.IsNullOrEmpty(invoice.CustomerEmail)) c.Item().Text($"E-Posta: {invoice.CustomerEmail}").FontSize(8);
                            if (!string.IsNullOrEmpty(invoice.CustomerPhone)) c.Item().Text($"Tel: {invoice.CustomerPhone}").FontSize(8);
                            if (!string.IsNullOrEmpty(invoice.CustomerTaxNumber)) c.Item().Text($"VKN: {invoice.CustomerTaxNumber}").FontSize(8);
                        });

                        // Metadata Box as a Column with Rows, Vertical Separator and Left-Aligned values
                        r.RelativeItem().AlignRight().PaddingRight(1).PaddingTop(5).Width(220).Border(0.5f).Column(mc =>
                        {
                            void AddMetaRow(string label, string value, bool last = false)
                            {
                                var rowItem = mc.Item();
                                if (!last) rowItem = rowItem.BorderBottom(0.5f);
                                
                                rowItem.Row(rowContent =>
                                {
                                    rowContent.RelativeItem(1.2f).PaddingVertical(2).PaddingHorizontal(4).Text(label).Bold().FontSize(8);
                                    rowContent.RelativeItem(1.8f).BorderLeft(0.5f).PaddingVertical(2).PaddingHorizontal(4).AlignLeft().Text(value).FontSize(8);
                                });
                            }

                            AddMetaRow("Özelleştirme No:", "TR1.2");
                            AddMetaRow("Senaryo:", invoice.Scenario == InvoiceScenario.TicariFatura ? "TICARIFATURA" : "TEMELFATURA");
                            AddMetaRow("Fatura Tipi:", invoice.InvoiceType == InvoiceType.Alis ? "ALIS" : "SATIS");
                            AddMetaRow("Fatura No:", invoice.InvoiceNumber);
                            AddMetaRow("Fatura Tarihi:", invoice.InvoiceDate.ToString("dd-MM-yyyy HH:mm"), true);
                        });
                    });

                    // Line shortened to left side (40%) - Increased thickness to 2.0f
                    col.Item().PaddingTop(10).Row(row => 
                    {
                        row.RelativeItem(4).LineHorizontal(2.0f).LineColor(Colors.Black);
                        row.RelativeItem(6);
                    });
                    col.Item().PaddingTop(2).Text(x => {
                        var ettnValue = invoice.Ettn;
                        x.Span("ETTN: ").Bold().FontSize(8);
                        x.Span(ettnValue).FontSize(8);
                    });

                    col.Item().PaddingTop(15).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(35);  // Sıra No
                            c.ConstantColumn(70);  // Stok Kodu
                            c.RelativeColumn();    // Mal Hizmet
                            c.ConstantColumn(45);  // Miktar
                            c.ConstantColumn(75);  // Birim Fiyat
                            c.ConstantColumn(45);  // KDV %
                            c.ConstantColumn(75);  // KDV Tutarı
                            c.ConstantColumn(85);  // Tutar
                        });
                        
                        t.Header(h =>
                        {
                            h.Cell().Element(HeaderStyle).Text("Sıra No");
                            h.Cell().Element(HeaderStyle).Text("Stok Kodu");
                            h.Cell().Element(HeaderStyle).Text("Mal Hizmet");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Miktar");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Birim Fiyat");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("KDV %");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("KDV Tutarı");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Tutar");
                        });

                        var i = 1;
                        foreach (var item in invoice.Items)
                        {
                            t.Cell().Element(CellStyle).Text(i++.ToString());
                            t.Cell().Element(CellStyle).Text(item.Product?.Code ?? "");
                            t.Cell().Element(CellStyle).Text(item.Description);
                            t.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N2"));
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice.ToString("G29")} TL");
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.VatRate}%");
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.LineVatAmount.ToString("N2")} TL");
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.LineTotalExclVat.ToString("N2")} TL");
                        }
                    });

                    // Totals Row
                    col.Item().PaddingTop(10).Row(r =>
                    {
                        r.RelativeItem().Text(""); // Empty space on the left

                        r.ConstantItem(240).Table(t =>
                        {
                            t.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(100); });
                            
                            t.Cell().Element(TotalStyle).Text("Mal Hizmet Toplam Tutarı:").Bold();
                            t.Cell().Element(TotalStyle).AlignRight().Text($"{invoice.SubTotal.ToString("N2")} TL");

                            t.Cell().Element(TotalStyle).Text("Hesaplanan KDV (%18):").Bold();
                            t.Cell().Element(TotalStyle).AlignRight().Text($"{invoice.TotalVat.ToString("N2")} TL");

                            t.Cell().Element(TotalStyle).Text("Vergiler Dahil Toplam Tutar:").Bold();
                            t.Cell().Element(TotalStyle).AlignRight().Text($"{invoice.GrandTotal.ToString("N2")} TL");

                            t.Cell().Element(TotalStyle).Text("Ödenecek Tutar:").Bold();
                            t.Cell().Element(TotalStyle).AlignRight().Text($"{invoice.GrandTotal.ToString("N2")} TL");
                        });
                    });

                    // Notes & IBAN Box (Bottom)
                    col.Item().PaddingTop(15).Border(0.5f).Padding(10).Column(c =>
                    {
                        c.Item().Text(AmountInWords(invoice.GrandTotal)).Italic().FontSize(9);
                        if (!string.IsNullOrEmpty(company?.IBAN))
                        {
                            c.Item().PaddingTop(8).Text(x =>
                            {
                                if (!string.IsNullOrEmpty(company.IBAN))
                                {
                                    x.Span("IBAN: ").Bold();
                                    x.Span(company.IBAN + "  ");
                                }
                                if (!string.IsNullOrEmpty(company.BankName))
                                {
                                    x.Span("Banka: ").Bold();
                                    x.Span(company.BankName + "  ");
                                }
                                if(!string.IsNullOrEmpty(company.IBAN) && !string.IsNullOrEmpty(company.IBAN))
                                {
                                    x.Span("Döviz Türü: ").Bold();
                                    x.Span("TRY");
                                }
                            });
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Sayfa ");
                    x.CurrentPageNumber();
                });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer HeaderStyle(IContainer c) => c.Border(0.5f).Padding(4).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.Bold());
    private static IContainer CellStyle(IContainer c) => c.Border(0.5f).Padding(4).AlignMiddle();
    private static IContainer MetaStyle(IContainer c) => c.PaddingHorizontal(4).PaddingVertical(2).BorderBottom(0.5f).BorderColor(Colors.Black);
    private static IContainer TotalStyle(IContainer c) => c.Border(0.5f).Padding(4);

    private static string AmountInWords(decimal sayi)
    {
        string[] birler = { "", "Bir", "İki", "Üç", "Dört", "Beş", "Altı", "Yedi", "Sekiz", "Dokuz" };
        string[] onlar = { "", "On", "Yirmi", "Otuz", "Kırk", "Elli", "Altmış", "Yetmiş", "Seksen", "Doksan" };
        string[] binler = { "", "Bin", "Milyon", "Milyar", "Trilyon" };

        string sonuc = "";
        long tamKisim = (long)Math.Floor(sayi);
        int kurusKisim = (int)Math.Round((sayi - tamKisim) * 100);

        if (tamKisim == 0) sonuc = "Sıfır";
        else
        {
            int grupSayisi = 0;
            while (tamKisim > 0)
            {
                int grup = (int)(tamKisim % 1000);
                if (grup > 0)
                {
                    string grupYazi = "";
                    int yuzlerBas = grup / 100;
                    int onlarBas = (grup % 100) / 10;
                    int birlerBas = grup % 10;

                    if (yuzlerBas > 0)
                    {
                        if (yuzlerBas > 1) grupYazi += birler[yuzlerBas];
                        grupYazi += "Yüz";
                    }
                    grupYazi += onlar[onlarBas];
                    if (grupSayisi == 1 && birlerBas == 1 && onlarBas == 0 && yuzlerBas == 0)
                    {
                        // "BirBin" yerine "Bin"
                    }
                    else
                    {
                        grupYazi += birler[birlerBas];
                    }

                    sonuc = grupYazi + binler[grupSayisi] + sonuc;
                }
                tamKisim /= 1000;
                grupSayisi++;
            }
        }

        sonuc += "TürkLirası";
        if (kurusKisim > 0)
        {
            sonuc += " " + onlar[kurusKisim / 10] + birler[kurusKisim % 10] + " Kuruş";
        }

        return "Yalnız " + sonuc;
    }

    private byte[] GenerateQrCode(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(10);
    }
}
