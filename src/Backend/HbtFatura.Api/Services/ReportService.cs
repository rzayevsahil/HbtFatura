using System.Collections.Generic;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;
using HbtFatura.Api.Constants;
using HbtFatura.Api.Data;
using HbtFatura.Api.DTOs.Reports;
using HbtFatura.Api.Entities;
//using InvoiceType = HbtFatura.Api.Entities.InvoiceType;

namespace HbtFatura.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserContext _currentUser;

    public ReportService(AppDbContext db, ICurrentUserContext currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private IQueryable<Customer> CustomerScope()
    {
        var q = _db.Customers.IgnoreQueryFilters().Where(x => !x.IsDeleted);
        if (_currentUser.IsSuperAdmin) return q;
        if (_currentUser.FirmId.HasValue)
            return q.Where(x => x.User != null && x.User.FirmId == _currentUser.FirmId.Value);
        return q.Where(x => x.UserId == _currentUser.UserId);
    }

    private IQueryable<CashRegister> CashScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.CashRegisters.AsQueryable();
        if (_currentUser.FirmId.HasValue)
            return _db.CashRegisters.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.CashRegisters.Where(x => false);
    }

    private IQueryable<BankAccount> BankScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.BankAccounts.AsQueryable();
        if (_currentUser.FirmId.HasValue)
            return _db.BankAccounts.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.BankAccounts.Where(x => false);
    }

    private IQueryable<Product> ProductScope(Guid? firmId)
    {
        var q = _db.Products.AsQueryable();
        if (_currentUser.IsSuperAdmin)
        {
            if (firmId.HasValue) return q.Where(x => x.FirmId == firmId.Value);
            return q;
        }
        if (_currentUser.FirmId.HasValue)
            return q.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return q.Where(x => false);
    }

    private IQueryable<Invoice> InvoiceScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.Invoices.AsQueryable();
        if (_currentUser.FirmId.HasValue)
            return _db.Invoices.Where(i => i.User != null && i.User.FirmId == _currentUser.FirmId.Value);
        return _db.Invoices.Where(i => i.UserId == _currentUser.UserId);
    }

    private IQueryable<Order> OrderScope(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.Orders.Where(o => o.User != null && o.User.FirmId == firmIdFilter.Value);
            return _db.Orders.AsQueryable();
        }
        if (_currentUser.FirmId.HasValue)
            return _db.Orders.Where(o => o.User != null && o.User.FirmId == _currentUser.FirmId.Value);
        return _db.Orders.Where(o => o.UserId == _currentUser.UserId);
    }
    
    private IQueryable<DeliveryNote> DeliveryNoteScope(Guid? firmIdFilter = null)
    {
        if (_currentUser.IsSuperAdmin)
        {
            if (firmIdFilter.HasValue)
                return _db.DeliveryNotes.Where(d => d.User != null && d.User.FirmId == firmIdFilter.Value);
            return _db.DeliveryNotes.AsQueryable();
        }
        if (_currentUser.FirmId.HasValue)
            return _db.DeliveryNotes.Where(d => d.User != null && d.User.FirmId == _currentUser.FirmId.Value);
        return _db.DeliveryNotes.Where(d => d.UserId == _currentUser.UserId);
    }

    public async Task<CariExtractReportDto?> GetCariExtractAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var customer = await CustomerScope().Where(c => c.Id == customerId)
            .Select(c => new { c.Id, c.Title })
            .FirstOrDefaultAsync(ct);
        if (customer == null) return null;

        var query = _db.AccountTransactions.Where(t => t.CustomerId == customerId);
        if (dateFrom.HasValue) query = query.Where(t => t.Date >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(t => t.Date <= dateTo.Value.Date);
        var transactions = await query.OrderBy(t => t.Date).ThenBy(t => t.CreatedAt)
            .Select(t => new { t.Date, t.Description, t.Type, t.Amount, t.Currency })
            .ToListAsync(ct);

        decimal openingBalance = 0;
        if (dateFrom.HasValue)
            openingBalance = await _db.AccountTransactions
                .Where(t => t.CustomerId == customerId && t.Date < dateFrom.Value.Date)
                .SumAsync(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount, ct);
        else if (transactions.Count > 0)
        {
            var first = transactions[0];
            openingBalance = await _db.AccountTransactions
                .Where(t => t.CustomerId == customerId && t.Date < first.Date)
                .SumAsync(t => t.Type == AccountTransactionType.Alacak ? t.Amount : -t.Amount, ct);
        }

        decimal running = openingBalance;
        var rows = new List<CariExtractRowDto>();
        foreach (var t in transactions)
        {
            var borc = t.Type == AccountTransactionType.Borc ? t.Amount : 0;
            var alacak = t.Type == AccountTransactionType.Alacak ? t.Amount : 0;
            running += alacak - borc;
            var rowCurrency = string.IsNullOrWhiteSpace(t.Currency) ? "TRY" : t.Currency.Trim();
            rows.Add(new CariExtractRowDto
            {
                Date = t.Date,
                Description = t.Description,
                Borc = borc,
                Alacak = alacak,
                Bakiye = running,
                Currency = rowCurrency
            });
        }

        var reportCurrency = rows.Count > 0 ? rows[0].Currency : "TRY";

        // Dönem boşsa verideki ilk/son tarihi kullan (PDF'de "—" yerine anlamlı dönem gösterilsin)
        DateTime? effectiveDateFrom = dateFrom;
        DateTime? effectiveDateTo = dateTo;
        if (rows.Count > 0)
        {
            if (!effectiveDateFrom.HasValue) effectiveDateFrom = rows.Min(r => r.Date).Date;
            if (!effectiveDateTo.HasValue) effectiveDateTo = rows.Max(r => r.Date).Date;
        }

        return new CariExtractReportDto
        {
            CustomerId = customer.Id,
            CustomerTitle = customer.Title,
            DateFrom = effectiveDateFrom,
            DateTo = effectiveDateTo,
            OpeningBalance = openingBalance,
            ClosingBalance = running,
            Currency = reportCurrency,
            Rows = rows
        };
    }

    public async Task<CashSummaryReportDto?> GetCashSummaryAsync(Guid? cashRegisterId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var registers = await CashScope().ToListAsync(ct);
        if (registers.Count == 0) return null;
        var regs = cashRegisterId.HasValue ? registers.Where(x => x.Id == cashRegisterId.Value).ToList() : registers;
        if (regs.Count == 0) return null;

        var result = new CashSummaryReportDto
        {
            CashRegisterId = regs.Count == 1 ? regs[0].Id : null,
            CashRegisterName = regs.Count == 1 ? regs[0].Name : null,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Rows = new List<CashSummaryRowDto>()
        };

        foreach (var reg in regs)
        {
            var q = _db.CashTransactions.Where(t => t.CashRegisterId == reg.Id);
            if (dateFrom.HasValue) q = q.Where(t => t.Date >= dateFrom.Value.Date);
            if (dateTo.HasValue) q = q.Where(t => t.Date <= dateTo.Value.Date);
            var list = await q.OrderBy(t => t.Date).Select(t => new { t.Date, t.Description, t.Type, t.Amount }).ToListAsync(ct);

            decimal opening = 0;
            if (dateFrom.HasValue)
                opening = await _db.CashTransactions
                    .Where(t => t.CashRegisterId == reg.Id && t.Date < dateFrom.Value.Date)
                    .SumAsync(t => t.Type == CashTransactionType.Giris ? t.Amount : -t.Amount, ct);

            result.OpeningBalance += opening;
            decimal totalGiris = 0, totalCikis = 0;
            foreach (var t in list)
            {
                if (t.Type == CashTransactionType.Giris) totalGiris += t.Amount; else totalCikis += t.Amount;
                result.Rows.Add(new CashSummaryRowDto { Date = t.Date, Description = t.Description, Type = t.Type, Amount = t.Amount });
            }
            result.TotalGiris += totalGiris;
            result.TotalCikis += totalCikis;
            result.ClosingBalance += opening + totalGiris - totalCikis;
        }
        return result;
    }

    public async Task<BankSummaryReportDto?> GetBankSummaryAsync(Guid? bankAccountId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var accounts = await BankScope().ToListAsync(ct);
        if (accounts.Count == 0) return null;
        var accts = bankAccountId.HasValue ? accounts.Where(x => x.Id == bankAccountId.Value).ToList() : accounts;
        if (accts.Count == 0) return null;

        var result = new BankSummaryReportDto
        {
            BankAccountId = accts.Count == 1 ? accts[0].Id : null,
            BankAccountName = accts.Count == 1 ? accts[0].Name : null,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Rows = new List<BankSummaryRowDto>()
        };

        foreach (var acc in accts)
        {
            var q = _db.BankTransactions.Where(t => t.BankAccountId == acc.Id);
            if (dateFrom.HasValue) q = q.Where(t => t.Date >= dateFrom.Value.Date);
            if (dateTo.HasValue) q = q.Where(t => t.Date <= dateTo.Value.Date);
            var list = await q.OrderBy(t => t.Date).Select(t => new { t.Date, t.Description, t.Type, t.Amount }).ToListAsync(ct);

            decimal opening = 0;
            if (dateFrom.HasValue)
                opening = await _db.BankTransactions
                    .Where(t => t.BankAccountId == acc.Id && t.Date < dateFrom.Value.Date)
                    .SumAsync(t => t.Type == BankTransactionType.Giris ? t.Amount : -t.Amount, ct);

            result.OpeningBalance += opening;
            decimal totalGiris = 0, totalCikis = 0;
            foreach (var t in list)
            {
                if (t.Type == BankTransactionType.Giris) totalGiris += t.Amount; else totalCikis += t.Amount;
                result.Rows.Add(new BankSummaryRowDto { Date = t.Date, Description = t.Description, Type = t.Type, Amount = t.Amount });
            }
            result.TotalGiris += totalGiris;
            result.TotalCikis += totalCikis;
            result.ClosingBalance += opening + totalGiris - totalCikis;
        }
        return result;
    }

    public async Task<StockLevelsReportDto> GetStockLevelsAsync(Guid? firmId, CancellationToken ct = default)
    {
        var products = await ProductScope(firmId)
            .OrderBy(x => x.Code)
            .Select(x => new { x.Id, x.Code, x.Name, x.Unit })
            .ToListAsync(ct);
        var ids = products.Select(x => x.Id).ToList();
        var stockDict = new Dictionary<Guid, decimal>();
        if (ids.Count > 0)
        {
            var stockList = await _db.StockMovements.Where(m => ids.Contains(m.ProductId))
                .GroupBy(m => m.ProductId)
                .Select(g => new { ProductId = g.Key, Qty = g.Sum(m => m.Type == StockMovementType.Giris ? m.Quantity : -m.Quantity) })
                .ToListAsync(ct);
            foreach (var x in stockList) stockDict[x.ProductId] = x.Qty;
        }

        var items = products.Select(p => new StockLevelRowDto
        {
            ProductId = p.Id,
            Code = p.Code,
            Name = p.Name,
            Unit = p.Unit,
            Quantity = stockDict.GetValueOrDefault(p.Id, 0)
        }).ToList();

        return new StockLevelsReportDto { Items = items };
    }

    public async Task<byte[]?> GetStockLevelsPdfAsync(Guid? firmId, CancellationToken ct = default)
    {
        var data = await GetStockLevelsAsync(firmId, ct);
        var unitLabels = await LookupMaps.LoadStringCodeMapAsync(_db, "ProductUnit", ct);
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text("Stok raporu").Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Tarih: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}");
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100);
                            c.RelativeColumn();
                            c.ConstantColumn(50);
                            c.ConstantColumn(80);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Kod").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Ad").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Birim").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Miktar").Bold();
                        });
                        foreach (var row in data.Items)
                        {
                            t.Cell().Padding(4).Text(row.Code);
                            t.Cell().Padding(4).Text(row.Name);
                            t.Cell().Padding(4).Text(LookupMaps.FormatStringCode(row.Unit, unitLabels));
                            t.Cell().Padding(4).AlignRight().Text(row.Quantity.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")));
                        }
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetCariExtractPdfAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var data = await GetCariExtractAsync(customerId, dateFrom, dateTo, ct);
        if (data == null) return null;
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text($"Cari ekstre - {data.CustomerTitle}").Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Dönem: {(data.DateFrom?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")} - {(data.DateTo?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")}");
                    col.Item().Text($"Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}");
                    col.Item().Text($"Açılış bakiyesi: {data.OpeningBalance:N2} {data.Currency}");
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        // Sütunları yeterince geniş tut: tarih, borç/alacak/bakiye tek satırda kalsın (kayma olmasın)
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100);  // Tarih (dd.MM.yyyy HH:mm)
                            c.RelativeColumn();
                            c.ConstantColumn(105);  // Borç (tutar + para birimi)
                            c.ConstantColumn(105);  // Alacak
                            c.ConstantColumn(105);  // Bakiye
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tarih").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Açıklama").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Borç").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Alacak").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Bakiye").Bold();
                        });
                        foreach (var row in data.Rows)
                        {
                            var cur = string.IsNullOrWhiteSpace(row.Currency) ? "TRY" : row.Currency;
                            var dateStr = row.Date.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"));
                            var borcStr = row.Borc.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")) + " " + cur;
                            var alacakStr = row.Alacak.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")) + " " + cur;
                            var bakiyeStr = row.Bakiye.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")) + " " + cur;
                            t.Cell().Padding(4).Text(dateStr);
                            t.Cell().Padding(4).Text(row.Description);
                            t.Cell().Padding(4).Text(borcStr);
                            t.Cell().Padding(4).Text(alacakStr);
                            t.Cell().Padding(4).Text(bakiyeStr);
                        }
                    });
                    col.Item().PaddingTop(8).Text($"Kapanış bakiyesi: {data.ClosingBalance:N2} {data.Currency}").Bold();
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetCariExtractExcelAsync(Guid customerId, DateTime? dateFrom, DateTime? dateTo, CancellationToken ct = default)
    {
        var data = await GetCariExtractAsync(customerId, dateFrom, dateTo, ct);
        if (data == null) return null;

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Cari ekstre");
        ws.Cell("A1").Value = $"Cari ekstre - {data.CustomerTitle}";
        ws.Cell("A2").Value = $"Dönem: {(data.DateFrom?.ToString("d") ?? "—")} - {(data.DateTo?.ToString("d") ?? "—")}";
        ws.Cell("A3").Value = $"Açılış bakiyesi: {data.OpeningBalance:N2} {data.Currency}";
        ws.Cell("A5").Value = "Tarih";
        ws.Cell("B5").Value = "Açıklama";
        ws.Cell("C5").Value = "Borç";
        ws.Cell("D5").Value = "Alacak";
        ws.Cell("E5").Value = "Bakiye";
        ws.Cell("F5").Value = "Para birimi";
        int row = 6;
        foreach (var r in data.Rows)
        {
            var cur = string.IsNullOrWhiteSpace(r.Currency) ? "TRY" : r.Currency;
            ws.Cell(row, 1).Value = r.Date.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"));
            ws.Cell(row, 2).Value = r.Description;
            ws.Cell(row, 3).Value = r.Borc;
            ws.Cell(row, 4).Value = r.Alacak;
            ws.Cell(row, 5).Value = r.Bakiye;
            ws.Cell(row, 6).Value = cur;
            row++;
        }
        ws.Cell(row, 1).Value = "Kapanış bakiyesi:";
        ws.Cell(row, 5).Value = data.ClosingBalance;
        ws.Cell(row, 6).Value = data.Currency;
        using var ms = new MemoryStream();
        wb.SaveAs(ms, false);
        return ms.ToArray();
    }

    public async Task<InvoiceReportDto> GetInvoiceReportAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default)
    {
        var query = InvoiceScope();
        if (dateFrom.HasValue) query = query.Where(x => x.InvoiceDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.InvoiceDate <= dateTo.Value.Date);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);

        var items = await query
            .OrderByDescending(x => x.InvoiceDate)
            .ThenByDescending(x => x.InvoiceNumber)
            .Select(x => new InvoiceReportRowDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                InvoiceDate = x.InvoiceDate,
                Status = (int)x.Status,
                CustomerTitle = x.CustomerTitle,
                GrandTotal = x.GrandTotal,
                Currency = x.Currency
            })
            .ToListAsync(ct);

        string? customerTitle = null;
        if (customerId.HasValue)
            customerTitle = await CustomerScope().Where(c => c.Id == customerId.Value).Select(c => c.Title).FirstOrDefaultAsync(ct);

        return new InvoiceReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CustomerId = customerId,
            CustomerTitle = customerTitle,
            Items = items
        };
    }

    public async Task<byte[]?> GetInvoiceReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default)
    {
        var data = await GetInvoiceReportAsync(dateFrom, dateTo, customerId, ct);
        QuestPDF.Settings.License = LicenseType.Community;

        var title = data.CustomerTitle != null ? $"Fatura raporu - {data.CustomerTitle}" : "Fatura raporu";
        var period = $"Dönem: {(data.DateFrom?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")} - {(data.DateTo?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")}  |  Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}";
        var statusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "InvoiceStatus", ct);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text(title).Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text(period);
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(150);   // Fatura No (tek satırda kalsın)
                            c.ConstantColumn(120);   // Tarih (dd.MM.yyyy HH:mm)
                            c.RelativeColumn();      // Cari
                            c.ConstantColumn(90);   // Durum
                            c.ConstantColumn(110);  // Toplam
                            c.ConstantColumn(50);   // Para
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Fatura No").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tarih").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Cari").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Durum").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Toplam").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Para").Bold();
                        });
                        var culture = CultureInfo.GetCultureInfo("tr-TR");
                        foreach (var row in data.Items)
                        {
                            t.Cell().Padding(4).Text(row.InvoiceNumber);
                            t.Cell().Padding(4).Text(row.InvoiceDate.ToString("dd.MM.yyyy HH:mm", culture));
                            t.Cell().Padding(4).Text(row.CustomerTitle);
                            t.Cell().Padding(4).Text(LookupMaps.FormatIntCode(row.Status, statusLabels));
                            t.Cell().Padding(4).AlignRight().Text(row.GrandTotal.ToString("N2", culture));
                            t.Cell().Padding(4).Text(row.Currency);
                        }
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetInvoiceReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, Guid? customerId, CancellationToken ct = default)
    {
        var data = await GetInvoiceReportAsync(dateFrom, dateTo, customerId, ct);
        var statusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "InvoiceStatus", ct);
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Fatura raporu");
        ws.Cell("A1").Value = data.CustomerTitle != null ? $"Fatura raporu - {data.CustomerTitle}" : "Fatura raporu";
        ws.Cell("A2").Value = $"Dönem: {(data.DateFrom?.ToString("d", culture) ?? "—")} - {(data.DateTo?.ToString("d", culture) ?? "—")}";
        ws.Cell("A3").Value = $"Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", culture)}";
        ws.Cell("A5").Value = "Fatura No";
        ws.Cell("B5").Value = "Tarih";
        ws.Cell("C5").Value = "Cari";
        ws.Cell("D5").Value = "Durum";
        ws.Cell("E5").Value = "Toplam";
        ws.Cell("F5").Value = "Para";
        int row = 6;
        foreach (var r in data.Items)
        {
            ws.Cell(row, 1).Value = r.InvoiceNumber;
            ws.Cell(row, 2).Value = r.InvoiceDate.ToString("dd.MM.yyyy HH:mm", culture);
            ws.Cell(row, 3).Value = r.CustomerTitle;
            ws.Cell(row, 4).Value = LookupMaps.FormatIntCode(r.Status, statusLabels);
            ws.Cell(row, 5).Value = r.GrandTotal;
            ws.Cell(row, 6).Value = r.Currency;
            row++;
        }
        using var ms = new MemoryStream();
        wb.SaveAs(ms, false);
        return ms.ToArray();
    }

    public async Task<MonthlyProductSalesReportDto> GetMonthlyProductSalesAsync(DateTime? dateFrom, DateTime? dateTo, Guid? productId, CancellationToken ct = default)
    {
        var invScope = InvoiceScope().Where(i => i.InvoiceType == InvoiceType.Satis
            && (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Paid));
        var query = _db.InvoiceItems
            .Where(i => i.ProductId != null)
            .Join(invScope, i => i.InvoiceId, inv => inv.Id, (i, inv) => new { i.ProductId, i.Quantity, inv.InvoiceDate });
        if (dateFrom.HasValue) query = query.Where(x => x.InvoiceDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.InvoiceDate <= dateTo.Value.Date);
        if (productId.HasValue) query = query.Where(x => x.ProductId == productId.Value);

        var grouped = await query
            .GroupBy(x => new { ProductId = x.ProductId!.Value, Year = x.InvoiceDate.Year, Month = x.InvoiceDate.Month })
            .Select(g => new { g.Key.ProductId, g.Key.Year, g.Key.Month, QuantitySold = g.Sum(x => x.Quantity) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.ProductId)
            .ToListAsync(ct);

        var ids = grouped.Select(x => x.ProductId).Distinct().ToList();
        var products = await ProductScope(null).Where(p => ids.Contains(p.Id)).Select(p => new { p.Id, p.Code, p.Name }).ToListAsync(ct);
        var productDict = products.ToDictionary(p => p.Id);

        var items = grouped.Select(g =>
        {
            productDict.TryGetValue(g.ProductId, out var p);
            return new MonthlyProductSalesRowDto
            {
                ProductId = g.ProductId,
                ProductCode = p?.Code ?? "",
                ProductName = p?.Name ?? "",
                Year = g.Year,
                Month = g.Month,
                QuantitySold = g.QuantitySold
            };
        }).ToList();

        return new MonthlyProductSalesReportDto { DateFrom = dateFrom, DateTo = dateTo, Items = items };
    }

    public async Task<OrderReportDto> GetOrderReportAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var query = OrderScope(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.OrderDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.OrderDate < dateTo.Value.Date.AddDays(1));
        if (status.HasValue)
        {
            var st = (OrderStatus)status.Value;
            query = query.Where(x => x.Status == st);
        }
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x => x.OrderNumber.ToLower().Contains(s) || (x.Customer != null && x.Customer.Title.ToLower().Contains(s)));
        }

        var items = await query
            .Include(x => x.Customer)
            .OrderByDescending(x => x.OrderDate)
            .ThenByDescending(x => x.OrderNumber)
            .Select(o => new OrderReportRowDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.OrderDate,
                Status = (int)o.Status,
                OrderType = (int)o.OrderType,
                CustomerId = o.CustomerId,
                CustomerTitle = o.Customer != null ? o.Customer.Title : null,
                TotalAmount = o.Items.Sum(i => i.Quantity * i.UnitPrice * (1 + i.VatRate / 100m)),
                Currency = "TRY"
            })
            .ToListAsync(ct);

        string? customerTitle = null;
        if (customerId.HasValue)
            customerTitle = await CustomerScope().Where(c => c.Id == customerId.Value).Select(c => c.Title).FirstOrDefaultAsync(ct);

        return new OrderReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CustomerId = customerId,
            CustomerTitle = customerTitle,
            Status = status,
            Search = search,
            FirmId = firmId,
            Items = items
        };
    }

    public async Task<byte[]?> GetOrderReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var data = await GetOrderReportAsync(dateFrom, dateTo, status, customerId, search, firmId, ct);
        QuestPDF.Settings.License = LicenseType.Community;

        var title = data.CustomerTitle != null ? $"Sipariş raporu - {data.CustomerTitle}" : "Sipariş raporu";
        var period = $"Dönem: {(data.DateFrom?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")} - {(data.DateTo?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")}  |  Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}";
        var orderStatusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "OrderStatus", ct);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text(title).Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text(period);
                    if (!string.IsNullOrWhiteSpace(data.Search))
                    {
                        col.Item().Text($"Arama: {data.Search}");
                    }
                    if (data.Status.HasValue)
                    {
                        col.Item().Text($"Durum: {LookupMaps.FormatIntCode(data.Status.Value, orderStatusLabels)}");
                    }
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            // Sipariş No ve Tarih kolonlarını geniş tut, satır kırılmasını azalt
                            c.ConstantColumn(150);   // Sipariş No
                            c.ConstantColumn(120);   // Tarih (dd.MM.yyyy HH:mm)
                            c.RelativeColumn();      // Cari
                            c.ConstantColumn(90);    // Durum
                            c.ConstantColumn(110);   // Toplam
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Sipariş No").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tarih").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Cari").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Durum").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Toplam").Bold();
                        });
                        var culture = CultureInfo.GetCultureInfo("tr-TR");
                        foreach (var row in data.Items)
                        {
                            t.Cell().Padding(4).Text(row.OrderNumber);
                            t.Cell().Padding(4).Text(row.OrderDate.ToString("dd.MM.yyyy HH:mm", culture));
                            t.Cell().Padding(4).Text(row.CustomerTitle ?? string.Empty);
                            t.Cell().Padding(4).Text(LookupMaps.FormatIntCode(row.Status, orderStatusLabels));
                            t.Cell().Padding(4).AlignRight().Text($"{row.TotalAmount.ToString("N2", culture)} {row.Currency}");
                        }
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetOrderReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var data = await GetOrderReportAsync(dateFrom, dateTo, status, customerId, search, firmId, ct);
        var orderStatusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "OrderStatus", ct);

        var culture = CultureInfo.GetCultureInfo("tr-TR");
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Sipariş raporu");
        ws.Cell("A1").Value = data.CustomerTitle != null ? $"Sipariş raporu - {data.CustomerTitle}" : "Sipariş raporu";
        ws.Cell("A2").Value = $"Dönem: {(data.DateFrom?.ToString("d", culture) ?? "—")} - {(data.DateTo?.ToString("d", culture) ?? "—")}";
        ws.Cell("A3").Value = $"Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", culture)}";
        var infoRow = 4;
        if (!string.IsNullOrWhiteSpace(data.Search))
        {
            ws.Cell(infoRow, 1).Value = $"Arama: {data.Search}";
            infoRow++;
        }
        if (data.Status.HasValue)
        {
            ws.Cell(infoRow, 1).Value = $"Durum: {LookupMaps.FormatIntCode(data.Status.Value, orderStatusLabels)}";
            infoRow++;
        }
        var headerRow = infoRow + 1;
        ws.Cell(headerRow, 1).Value = "Sipariş No";
        ws.Cell(headerRow, 2).Value = "Tarih";
        ws.Cell(headerRow, 3).Value = "Cari";
        ws.Cell(headerRow, 4).Value = "Durum";
        ws.Cell(headerRow, 5).Value = "Toplam";
        ws.Cell(headerRow, 6).Value = "Para";

        var row = headerRow + 1;
        foreach (var r in data.Items)
        {
            ws.Cell(row, 1).Value = r.OrderNumber;
            ws.Cell(row, 2).Value = r.OrderDate.ToString("dd.MM.yyyy HH:mm", culture);
            ws.Cell(row, 3).Value = r.CustomerTitle;
            ws.Cell(row, 4).Value = LookupMaps.FormatIntCode(r.Status, orderStatusLabels);
            ws.Cell(row, 5).Value = r.TotalAmount;
            ws.Cell(row, 6).Value = r.Currency;
            row++;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms, false);
        return ms.ToArray();
    }
    
    public async Task<DeliveryNoteReportDto> GetDeliveryNoteReportAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var query = DeliveryNoteScope(firmId);
        if (dateFrom.HasValue) query = query.Where(x => x.DeliveryDate >= dateFrom.Value.Date);
        if (dateTo.HasValue) query = query.Where(x => x.DeliveryDate <= dateTo.Value.Date);
        if (status.HasValue)
        {
            var st = (DeliveryNoteStatus)status.Value;
            query = query.Where(x => x.Status == st);
        }
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(x => x.DeliveryNumber.ToLower().Contains(s) || (x.Customer != null && x.Customer.Title.ToLower().Contains(s)));
        }

        var items = await query
            .Include(x => x.Customer)
            .Include(x => x.Order)
            .Include(x => x.Items)
            .OrderByDescending(x => x.DeliveryDate)
            .ThenByDescending(x => x.DeliveryNumber)
            .Select(d => new DeliveryNoteReportRowDto
            {
                Id = d.Id,
                DeliveryNumber = d.DeliveryNumber,
                DeliveryDate = d.DeliveryDate,
                Status = (int)d.Status,
                DeliveryType = (int)d.DeliveryType,
                CustomerId = d.CustomerId,
                CustomerTitle = d.Customer != null ? d.Customer.Title : null,
                OrderNumber = d.Order != null ? d.Order.OrderNumber : null,
                InvoiceId = d.InvoiceId,
                TotalAmount = d.Items.Sum(i => i.Quantity * i.UnitPrice * (1 + i.VatRate / 100m)),
                Currency = "TRY"
            })
            .ToListAsync(ct);

        string? customerTitle = null;
        if (customerId.HasValue)
            customerTitle = await CustomerScope().Where(c => c.Id == customerId.Value).Select(c => c.Title).FirstOrDefaultAsync(ct);

        return new DeliveryNoteReportDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            CustomerId = customerId,
            CustomerTitle = customerTitle,
            Status = status,
            Search = search,
            FirmId = firmId,
            Items = items
        };
    }

    public async Task<byte[]?> GetDeliveryNoteReportPdfAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var data = await GetDeliveryNoteReportAsync(dateFrom, dateTo, status, customerId, search, firmId, ct);
        QuestPDF.Settings.License = LicenseType.Community;

        var title = data.CustomerTitle != null ? $"İrsaliye raporu - {data.CustomerTitle}" : "İrsaliye raporu";
        var period = $"Dönem: {(data.DateFrom?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")} - {(data.DateTo?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")}  |  Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("tr-TR"))}";
        var dnStatusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "DeliveryNoteStatus", ct);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);
                page.Header().Text(title).Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text(period);
                    if (!string.IsNullOrWhiteSpace(data.Search))
                    {
                        col.Item().Text($"Arama: {data.Search}");
                    }
                    if (data.Status.HasValue)
                    {
                        col.Item().Text($"Durum: {LookupMaps.FormatIntCode(data.Status.Value, dnStatusLabels)}");
                    }
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            // İrsaliye No, Tarih ve Sipariş No için daha geniş kolonlar
                            c.ConstantColumn(150);   // İrsaliye No
                            c.ConstantColumn(120);   // Tarih (dd.MM.yyyy HH:mm)
                            c.RelativeColumn();      // Cari
                            c.ConstantColumn(90);    // Durum
                            c.ConstantColumn(140);   // Sipariş No
                            c.ConstantColumn(110);   // Toplam
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("İrsaliye No").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tarih").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Cari").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Durum").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Sipariş No").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Toplam").Bold();
                        });
                        var culture = CultureInfo.GetCultureInfo("tr-TR");
                        foreach (var row in data.Items)
                        {
                            t.Cell().Padding(4).Text(row.DeliveryNumber);
                            t.Cell().Padding(4).Text(row.DeliveryDate.ToString("dd.MM.yyyy HH:mm", culture));
                            t.Cell().Padding(4).Text(row.CustomerTitle ?? string.Empty);
                            t.Cell().Padding(4).Text(LookupMaps.FormatIntCode(row.Status, dnStatusLabels));
                            t.Cell().Padding(4).Text(row.OrderNumber ?? string.Empty);
                            t.Cell().Padding(4).AlignRight().Text($"{row.TotalAmount.ToString("N2", culture)} {row.Currency}");
                        }
                    });
                });
            });
        });
        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetDeliveryNoteReportExcelAsync(DateTime? dateFrom, DateTime? dateTo, int? status, Guid? customerId, string? search, Guid? firmId, CancellationToken ct = default)
    {
        var data = await GetDeliveryNoteReportAsync(dateFrom, dateTo, status, customerId, search, firmId, ct);
        var dnStatusLabels = await LookupMaps.LoadIntCodeMapAsync(_db, "DeliveryNoteStatus", ct);

        var culture = CultureInfo.GetCultureInfo("tr-TR");
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("İrsaliye raporu");
        ws.Cell("A1").Value = data.CustomerTitle != null ? $"İrsaliye raporu - {data.CustomerTitle}" : "İrsaliye raporu";
        ws.Cell("A2").Value = $"Dönem: {(data.DateFrom?.ToString("d", culture) ?? "—")} - {(data.DateTo?.ToString("d", culture) ?? "—")}";
        ws.Cell("A3").Value = $"Rapor tarihi: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", culture)}";
        var infoRow = 4;
        if (!string.IsNullOrWhiteSpace(data.Search))
        {
            ws.Cell(infoRow, 1).Value = $"Arama: {data.Search}";
            infoRow++;
        }
        if (data.Status.HasValue)
        {
            ws.Cell(infoRow, 1).Value = $"Durum: {LookupMaps.FormatIntCode(data.Status.Value, dnStatusLabels)}";
            infoRow++;
        }
        var headerRow = infoRow + 1;
        ws.Cell(headerRow, 1).Value = "İrsaliye No";
        ws.Cell(headerRow, 2).Value = "Tarih";
        ws.Cell(headerRow, 3).Value = "Cari";
        ws.Cell(headerRow, 4).Value = "Durum";
        ws.Cell(headerRow, 5).Value = "Sipariş No";
        ws.Cell(headerRow, 6).Value = "Toplam";
        ws.Cell(headerRow, 7).Value = "Para";

        var row = headerRow + 1;
        foreach (var r in data.Items)
        {
            ws.Cell(row, 1).Value = r.DeliveryNumber;
            ws.Cell(row, 2).Value = r.DeliveryDate.ToString("dd.MM.yyyy HH:mm", culture);
            ws.Cell(row, 3).Value = r.CustomerTitle;
            ws.Cell(row, 4).Value = LookupMaps.FormatIntCode(r.Status, dnStatusLabels);
            ws.Cell(row, 5).Value = r.OrderNumber;
            ws.Cell(row, 6).Value = r.TotalAmount;
            ws.Cell(row, 7).Value = r.Currency;
            row++;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms, false);
        return ms.ToArray();
    }

    public async Task<byte[]?> GetOrderDetailPdfAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await OrderScope(null)
            .Include(o => o.Customer)
            .Include(o => o.Items.OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order == null) return null;

        QuestPDF.Settings.License = LicenseType.Community;
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Header().Text($"Sipariş - {order.OrderNumber}").Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Tarih: {order.OrderDate.ToString("dd.MM.yyyy", culture)}").FontSize(10);
                    col.Item().Text($"Cari: {order.Customer?.Title ?? string.Empty}").FontSize(10);
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.ConstantColumn(70);
                            c.RelativeColumn();
                            c.ConstantColumn(50);
                            c.ConstantColumn(70);
                            c.ConstantColumn(55);
                            c.ConstantColumn(65);
                            c.ConstantColumn(85);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("#").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Stok Kodu").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Mal Hizmet").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Miktar").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Birim Fiyat").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("KDV Oranı").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("KDV Tutarı").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Mal Hizmet Tutarı").Bold().FontSize(9);
                        });
                        var index = 1;
                        decimal totalNet = 0;
                        decimal totalVat = 0;
                        foreach (var it in order.Items.OrderBy(i => i.SortOrder))
                        {
                            var qty = it.Quantity;
                            var unitPrice = it.UnitPrice;
                            var lineNet = qty * unitPrice;
                            var lineVat = lineNet * it.VatRate / 100m;
                            var lineTotal = lineNet + lineVat;
                            totalNet += lineNet;
                            totalVat += lineVat;
                            
                            t.Cell().Padding(3).Text(index.ToString()).FontSize(9);
                            t.Cell().Padding(3).Text(it.Product?.Code ?? "—").FontSize(9);
                            t.Cell().Padding(3).Text(it.Description ?? "—").FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(qty.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(unitPrice.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text($"{it.VatRate:N0}%").FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(lineVat.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(lineTotal.ToString("N2", culture)).FontSize(9);
                            index++;
                        }
                        var totalGross = totalNet + totalVat;

                        // Boş satır
                        t.Cell().ColumnSpan(8).PaddingTop(8).Text(string.Empty);

                        // Mal Hizmet Toplam Tutarı
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Mal Hizmet Toplam Tutarı:").Bold().FontSize(9);
                        t.Cell().AlignRight().Text($"{totalNet:N2} ₺").Bold().FontSize(9);

                        // Hesaplanan KDV
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Hesaplanan KDV:").Bold().FontSize(9);
                        t.Cell().AlignRight().Text($"{totalVat:N2} ₺").Bold().FontSize(9);

                        // Genel Toplam
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Genel Toplam:").Bold().FontColor(Color.FromHex("#059669")).FontSize(9);
                        t.Cell().AlignRight().Text($"{totalGross:N2} ₺").Bold().FontColor(Color.FromHex("#059669")).FontSize(9);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]?> GetDeliveryNoteDetailPdfAsync(Guid deliveryNoteId, CancellationToken ct = default)
    {
        var dn = await DeliveryNoteScope(null)
            .Include(d => d.Customer)
            .Include(d => d.Items.OrderBy(i => i.SortOrder))
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(d => d.Id == deliveryNoteId, ct);
        if (dn == null) return null;

        QuestPDF.Settings.License = LicenseType.Community;
        var culture = CultureInfo.GetCultureInfo("tr-TR");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.Header().Text($"İrsaliye - {dn.DeliveryNumber}").Bold().FontSize(14);
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Tarih: {dn.DeliveryDate.ToString("dd.MM.yyyy", culture)}").FontSize(10);
                    col.Item().Text($"Cari: {dn.Customer?.Title ?? string.Empty}").FontSize(10);
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.ConstantColumn(70);
                            c.RelativeColumn();
                            c.ConstantColumn(50);
                            c.ConstantColumn(70);
                            c.ConstantColumn(55);
                            c.ConstantColumn(65);
                            c.ConstantColumn(85);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("#").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Stok Kodu").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Mal Hizmet").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Miktar").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Birim Fiyat").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("KDV Oranı").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("KDV Tutarı").Bold().FontSize(9);
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Mal Hizmet Tutarı").Bold().FontSize(9);
                        });
                        var index = 1;
                        decimal totalNet = 0;
                        decimal totalVat = 0;
                        foreach (var it in dn.Items.OrderBy(i => i.SortOrder))
                        {
                            var qty = it.Quantity;
                            var unitPrice = it.UnitPrice;
                            var lineNet = qty * unitPrice;
                            var lineVat = lineNet * it.VatRate / 100m;
                            var lineTotal = lineNet + lineVat;
                            totalNet += lineNet;
                            totalVat += lineVat;
                            
                            t.Cell().Padding(3).Text(index.ToString()).FontSize(9);
                            t.Cell().Padding(3).Text(it.Product?.Code ?? "—").FontSize(9);
                            t.Cell().Padding(3).Text(it.Description ?? "—").FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(qty.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(unitPrice.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text($"{it.VatRate:N0}%").FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(lineVat.ToString("N2", culture)).FontSize(9);
                            t.Cell().Padding(3).AlignRight().Text(lineTotal.ToString("N2", culture)).FontSize(9);
                            index++;
                        }
                        var totalGross = totalNet + totalVat;

                        // Boş satır
                        t.Cell().ColumnSpan(8).PaddingTop(8).Text(string.Empty);

                        // Mal Hizmet Toplam Tutarı
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Mal Hizmet Toplam Tutarı:").Bold().FontSize(9);
                        t.Cell().AlignRight().Text($"{totalNet:N2} ₺").Bold().FontSize(9);

                        // Hesaplanan KDV
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Hesaplanan KDV:").Bold().FontSize(9);
                        t.Cell().AlignRight().Text($"{totalVat:N2} ₺").Bold().FontSize(9);

                        // Genel Toplam
                        t.Cell().ColumnSpan(5).Text(string.Empty);
                        t.Cell().ColumnSpan(2).AlignRight().Text("Genel Toplam:").Bold().FontColor(Color.FromHex("#059669")).FontSize(9);
                        t.Cell().AlignRight().Text($"{totalGross:N2} ₺").Bold().FontColor(Color.FromHex("#059669")).FontSize(9);
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}
