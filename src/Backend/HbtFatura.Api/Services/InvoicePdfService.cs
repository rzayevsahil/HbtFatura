using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using HbtFatura.Api.Data;
using HbtFatura.Api.Entities;
using QRCoder;

namespace HbtFatura.Api.Services;

public class InvoicePdfService : IInvoicePdfService
{
    private const string VatRateLookupGroupName = "VatRate";

    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;

    static InvoicePdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public InvoicePdfService(AppDbContext db, ICurrentUserContext currentUser, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IConfiguration configuration)
    {
        _db = db;
        _currentUser = currentUser;
        _env = env;
        _configuration = configuration;
    }

    /// <summary>Sistem tanımları (VatRate lookup) veya App:DefaultVatRate ile aynı kural — PDF etiketindeki oran metni.</summary>
    private async Task<string> GetSystemVatRateDisplayAsync(CancellationToken ct)
    {
        var code = await _db.Lookups
            .AsNoTracking()
            .Include(x => x.Group)
            .Where(x => x.Group != null && x.Group.Name == VatRateLookupGroupName && x.IsActive)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.Code)
            .FirstOrDefaultAsync(ct);

        if (!string.IsNullOrWhiteSpace(code)
            && decimal.TryParse(code.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed)
            && parsed is >= 0 and <= 100)
        {
            return parsed == decimal.Truncate(parsed)
                ? ((int)parsed).ToString(CultureInfo.InvariantCulture)
                : parsed.ToString("0.##", CultureInfo.InvariantCulture);
        }

        var fallback = _configuration.GetValue("App:DefaultVatRate", 20);
        if (fallback is < 0 or > 100)
            fallback = 20;
        return fallback.ToString(CultureInfo.InvariantCulture);
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
            company = await _db.CompanySettings
                .Include(x => x.TaxOffice)
                    .ThenInclude(t => t!.City)
                .Include(x => x.TaxOffice)
                    .ThenInclude(t => t!.District)
                .FirstOrDefaultAsync(x => x.FirmId == firmId.Value, ct);

        var systemVatRateDisplay = await GetSystemVatRateDisplayAsync(ct);

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
                    col.Item().Row(r =>
                    {
                        // 1. Company Info (Left) - INCLUDING the 40% top line
                        r.RelativeItem(4).Column(c =>
                        {
                            c.Item().LineHorizontal(2.0f).LineColor(Colors.Black);
                            
                            c.Item().PaddingTop(5).Column(inner => 
                            {
                                inner.Item().Text(company?.CompanyName ?? "Firma Adı").FontSize(8);
                                
                                var addressLines = new List<string>();
                                if (!string.IsNullOrEmpty(company?.Address)) addressLines.Add(company.Address);
                                
                                var cityDistrict = "";
                                if (company?.TaxOffice?.District != null) cityDistrict += company.TaxOffice.District.Name;
                                if (company?.TaxOffice?.City != null) cityDistrict += (cityDistrict != "" ? " / " : "") + company.TaxOffice.City.Name;
                                if (!string.IsNullOrEmpty(cityDistrict)) addressLines.Add(cityDistrict);
                                
                                foreach(var line in addressLines) inner.Item().Text(line).FontSize(8);

                                if (!string.IsNullOrEmpty(company?.Website)) inner.Item().Text($"Web: {company.Website}").FontSize(8);
                                if (!string.IsNullOrEmpty(company?.Email)) inner.Item().Text($"E-Posta: {company.Email}").FontSize(8);
                                if (!string.IsNullOrEmpty(company?.Phone)) inner.Item().Text($"Tel: {company.Phone}").FontSize(8);
                                if (!string.IsNullOrEmpty(company?.TaxOffice?.Name)) inner.Item().Text($"Vergi Dairesi: {company.TaxOffice.Name}").FontSize(8);
                                if (!string.IsNullOrEmpty(company?.TaxNumber)) 
                                {
                                    var label = (company.TaxNumber.Length == 11) ? "TCKN" : "VKN";
                                    inner.Item().Text($"{label}: {company.TaxNumber}").FontSize(8);
                                }
                            });

                            // Add bottom line for Sender Info
                            c.Item().PaddingTop(5).LineHorizontal(2.0f).LineColor(Colors.Black);
                        });

                        // 2. Logo & e-FATURA (Center)
                        r.RelativeItem(2).TranslateY(20).Column(c =>
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
                                // Sadece yükseklik: kare veya dikdörtgen GİB görseli oranı korur; ek Width ile çakışma yok
                                c.Item().AlignCenter().Height(60).Image(gibLogoPath).FitHeight();
                            }
                            else
                            {
                                c.Item().AlignCenter().Height(40).Text("GİB LOGO").FontSize(10).Italic();
                            }
                            c.Item().PaddingTop(5).AlignCenter().Text("e-FATURA").Bold().FontSize(10);
                        });

                        // 3. QR Code & Logo (Right) - Extreme top-right
                        r.RelativeItem(4).AlignRight().AlignTop().Row(row =>
                        {
                            if (!string.IsNullOrEmpty(company?.LogoUrl))
                            {
                                try
                                {
                                    // Satırda sabit sütun genişliği; içeride yalnızca yükseklik + FitArea (üstten genişlik ConstantItem’dan gelir).
                                    // ConstantItem + ayrı Width(aynı) + AlignMiddle() QuestPDF’te çakışıp kare logolarda da bozabiliyordu.
                                    const int firmLogoColW = 130;
                                    const int firmLogoMaxH = 72;

                                    if (company.LogoUrl.StartsWith("/"))
                                    {
                                        var relative = company.LogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                                        var webRoot = _env.WebRootPath;
                                        var logoPath = string.IsNullOrEmpty(webRoot)
                                            ? Path.Combine(_env.ContentRootPath, "wwwroot", relative)
                                            : Path.Combine(webRoot, relative);
                                        if (File.Exists(logoPath))
                                        {
                                            row.ConstantItem(firmLogoColW).PaddingRight(8).AlignTop().Column(logoCol =>
                                            {
                                                logoCol.Item().Height(firmLogoMaxH).AlignCenter()
                                                    .Image(logoPath).FitArea();
                                            });
                                        }
                                    }
                                    else
                                    {
                                        var base64Data = company.LogoUrl;
                                        if (base64Data.Contains(",")) base64Data = base64Data.Split(',')[1];
                                        var imageBytes = Convert.FromBase64String(base64Data);
                                        row.ConstantItem(firmLogoColW).PaddingRight(8).AlignTop().Column(logoCol =>
                                        {
                                            logoCol.Item().Height(firmLogoMaxH).AlignCenter()
                                                .Image(imageBytes).FitArea();
                                        });
                                    }
                                }
                                catch { }
                            }
                            // Generate rich QR data for a professional look (GIB standard-like)
                        var qrData = string.Join('|', 
                            company?.TaxNumber ?? "",
                            invoice.CustomerTaxNumber ?? "",
                            invoice.InvoiceNumber ?? "",
                            invoice.InvoiceDate.ToString("yyyy-MM-dd"),
                            invoice.SubTotal.ToString("F2"),
                            "TRY",
                            invoice.Ettn ?? "");

                        row.ConstantItem(70).AlignTop().Image(GenerateQrCode(qrData));
                        });
                    });
                });

                // --- CONTENT SECTION ---
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Sayın (Customer Info) + Metadata Table (Restructured)
                    col.Item().PaddingTop(5).Row(r =>
                    {
                        // Left column takes 40% to match header company info width
                        r.RelativeItem(4).PaddingTop(25).Column(c =>
                        {
                            // Top line - expand to full 40% column width
                            c.Item().LineHorizontal(2.0f).LineColor(Colors.Black);

                            c.Item().PaddingVertical(5).Column(inner =>
                            {
                                inner.Item().Text("SAYIN").Bold().FontSize(8);
                                inner.Item().Text(invoice.CustomerTitle).FontSize(8);
                                inner.Item().Text(invoice.CustomerAddress ?? "").FontSize(8);
                                
                                var customerCityDistrict = "";
                                if (!string.IsNullOrEmpty(invoice.CustomerDistrict)) customerCityDistrict += invoice.CustomerDistrict;
                                if (!string.IsNullOrEmpty(invoice.CustomerCity)) customerCityDistrict += (customerCityDistrict != "" ? " / " : "") + invoice.CustomerCity;
                                if (!string.IsNullOrEmpty(customerCityDistrict)) inner.Item().Text(customerCityDistrict).FontSize(8);
                                
                                if (!string.IsNullOrEmpty(invoice.CustomerWebsite)) inner.Item().Text($"Web: {invoice.CustomerWebsite}").FontSize(8);
                                if (!string.IsNullOrEmpty(invoice.CustomerEmail)) inner.Item().Text($"E-Posta: {invoice.CustomerEmail}").FontSize(8);
                                if (!string.IsNullOrEmpty(invoice.CustomerPhone)) inner.Item().Text($"Tel: {invoice.CustomerPhone}").FontSize(8);
                                if (!string.IsNullOrEmpty(invoice.CustomerTaxOffice)) inner.Item().Text($"Vergi Dairesi: {invoice.CustomerTaxOffice}").FontSize(8);
                                if (!string.IsNullOrEmpty(invoice.CustomerTaxNumber)) 
                                {
                                    var label = (invoice.CustomerTaxNumber.Length == 11) ? "TCKN" : "VKN";
                                    inner.Item().Text($"{label}: {invoice.CustomerTaxNumber}").FontSize(8);
                                }
                            });

                            // Bottom line - expand to full 40% column width
                            c.Item().LineHorizontal(2.0f).LineColor(Colors.Black);
                        });

                        // Right column takes rest (60%) and aligns metadata to extreme right - moved down with PaddingTop(40)
                        r.RelativeItem(6).AlignRight().PaddingRight(1).PaddingTop(40).Width(180).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(1.2f);
                                c.RelativeColumn(1.2f);
                            });

                            void AddMetaTableCelles(string label, string value)
                            {
                                t.Cell().Border(0.5f).PaddingVertical(2).PaddingHorizontal(4).Text(label).Bold().FontSize(8);
                                t.Cell().Border(0.5f).PaddingVertical(2).PaddingHorizontal(4).AlignLeft().Text(value).FontSize(8);
                            }

                            AddMetaTableCelles("Özelleştirme No:", "TR1.2");
                            AddMetaTableCelles("Senaryo:", invoice.Scenario == InvoiceScenario.TicariFatura ? "TICARIFATURA" : "TEMELFATURA");
                            AddMetaTableCelles("Fatura Tipi:", invoice.InvoiceType == InvoiceType.Alis ? "ALIS" : "SATIS");
                            AddMetaTableCelles("Fatura No:", invoice.InvoiceNumber);
                            AddMetaTableCelles("Fatura Tarihi:", invoice.InvoiceDate.ToString("dd-MM-yyyy HH:mm"));
                        });
                    });

                    col.Item().PaddingTop(6).Text(x => {
                        var ettnValue = invoice.Ettn;
                        x.Span("ETTN: ").Bold().FontSize(8);
                        x.Span(ettnValue).FontSize(8);
                    });

                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(30);  // Sıra No
                            c.ConstantColumn(65);  // Stok Kodu
                            c.RelativeColumn();    // Mal Hizmet
                            c.ConstantColumn(55);  // Miktar (+ birim tek satırda)
                            c.ConstantColumn(75);  // Birim Fiyat
                            c.ConstantColumn(60);  // KDV Oranı
                            c.ConstantColumn(75);  // KDV Tutarı
                            c.ConstantColumn(110); // Mal Hizmet Tutarı
                        });
                        
                        t.Header(h =>
                        {
                            h.Cell().Element(HeaderStyle).Text("Sıra No");
                            h.Cell().Element(HeaderStyle).Text("Stok Kodu");
                            h.Cell().Element(HeaderStyle).Text("Mal Hizmet");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Miktar");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Birim Fiyat");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("KDV Oranı");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("KDV Tutarı");
                            h.Cell().Element(HeaderStyle).AlignRight().Text("Mal Hizmet Tutarı");
                        });

                        var i = 1;
                        foreach (var item in invoice.Items)
                        {
                            t.Cell().Element(CellStyle).Text(i++.ToString());
                            t.Cell().Element(CellStyle).Text(item.Product?.Code ?? "");
                            t.Cell().Element(CellStyle).Text(item.Description);
                            t.Cell().Element(CellStyle).AlignRight().Text(FormatQuantityWithUnitForPdf(item.Quantity, item.Unit));
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.UnitPrice.ToString("G29")} TL");
                            t.Cell().Element(CellStyle).AlignRight().Text($"%{item.VatRate}");
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.LineVatAmount.ToString("N2")} TL");
                            t.Cell().Element(CellStyle).AlignRight().Text($"{item.LineTotalExclVat.ToString("N2")} TL");
                        }
                    });

                    // Totals Row
                    col.Item().PaddingTop(10).Row(r =>
                    {
                        r.RelativeItem().Text(""); // Empty space on the left

                        r.ConstantItem(245).Table(t =>
                        {
                            t.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(110); });
                            
                            t.Cell().Element(TotalStyle).Text("Mal Hizmet Toplam Tutarı:").Bold();
                            t.Cell().Element(TotalStyle).AlignRight().Text($"{invoice.SubTotal.ToString("N2")} TL");

                            t.Cell().Element(TotalStyle).Text($"Hesaplanan KDV (%{systemVatRateDisplay}):").Bold();
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
                        c.Item().Text(AmountInWords(invoice.GrandTotal)).Italic().FontSize(8);
                        if (!string.IsNullOrEmpty(company?.IBAN))
                        {
                            c.Item().PaddingTop(8).Text(x =>
                            {
                                x.DefaultTextStyle(ts => ts.FontSize(8));
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
                                if(!string.IsNullOrEmpty(company.IBAN))
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

    /// <summary>Miktar ve birimi tek satırda tutmak için normal boşluk yerine satır kırılmayan boşluk (NBSP).</summary>
    private static string FormatQuantityWithUnitForPdf(decimal quantity, string? unit)
    {
        var u = string.IsNullOrWhiteSpace(unit) ? "Adet" : unit.Trim();
        return $"{quantity.ToString("N2")}\u00A0{u}";
    }

    private static IContainer HeaderStyle(IContainer c) => c.Border(0.5f).Padding(4).AlignCenter().AlignMiddle().DefaultTextStyle(x => x.Bold().FontSize(8));
    private static IContainer CellStyle(IContainer c) => c.Border(0.5f).Padding(4).AlignMiddle().DefaultTextStyle(x => x.FontSize(8));
    private static IContainer MetaStyle(IContainer c) => c.PaddingHorizontal(4).PaddingVertical(2).BorderBottom(0.5f).BorderColor(Colors.Black);
    private static IContainer TotalStyle(IContainer c) => c.Border(0.5f).Padding(4).DefaultTextStyle(x => x.FontSize(8));

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
