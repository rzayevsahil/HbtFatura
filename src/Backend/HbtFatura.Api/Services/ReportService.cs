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
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return q.Where(x => x.User != null && x.User.FirmId == _currentUser.FirmId);
        return q.Where(x => x.UserId == _currentUser.UserId);
    }

    private IQueryable<CashRegister> CashScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.CashRegisters.AsQueryable();
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.CashRegisters.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return _db.CashRegisters.Where(x => false);
    }

    private IQueryable<BankAccount> BankScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.BankAccounts.AsQueryable();
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
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
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return q.Where(x => x.FirmId == _currentUser.FirmId.Value);
        return q.Where(x => false);
    }

    private IQueryable<Invoice> InvoiceScope()
    {
        if (_currentUser.IsSuperAdmin) return _db.Invoices.AsQueryable();
        if (_currentUser.IsFirmAdmin && _currentUser.FirmId.HasValue)
            return _db.Invoices.Where(i => i.User != null && i.User.FirmId == _currentUser.FirmId.Value);
        return _db.Invoices.Where(i => i.UserId == _currentUser.UserId);
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
            .Select(t => new { t.Date, t.Description, t.Type, t.Amount })
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
            rows.Add(new CariExtractRowDto
            {
                Date = t.Date,
                Description = t.Description,
                Borc = borc,
                Alacak = alacak,
                Bakiye = running
            });
        }

        return new CariExtractReportDto
        {
            CustomerId = customer.Id,
            CustomerTitle = customer.Title,
            DateFrom = dateFrom,
            DateTo = dateTo,
            OpeningBalance = openingBalance,
            ClosingBalance = running,
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
                    col.Item().Text($"Dönem: {(data.DateFrom?.ToString("d") ?? "—")} - {(data.DateTo?.ToString("d") ?? "—")}");
                    col.Item().Text($"Açılış bakiyesi: {data.OpeningBalance:N2} ₺");
                    col.Item().PaddingTop(10).Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(70);
                            c.RelativeColumn();
                            c.ConstantColumn(80);
                            c.ConstantColumn(80);
                            c.ConstantColumn(90);
                        });
                        t.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Tarih").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text("Açıklama").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Borç").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Alacak").Bold();
                            h.Cell().Background(Colors.Grey.Lighten2).Padding(4).AlignRight().Text("Bakiye").Bold();
                        });
                        foreach (var row in data.Rows)
                        {
                            t.Cell().Padding(4).Text(row.Date.ToString("d"));
                            t.Cell().Padding(4).Text(row.Description);
                            t.Cell().Padding(4).AlignRight().Text(row.Borc.ToString("N2"));
                            t.Cell().Padding(4).AlignRight().Text(row.Alacak.ToString("N2"));
                            t.Cell().Padding(4).AlignRight().Text(row.Bakiye.ToString("N2"));
                        }
                    });
                    col.Item().PaddingTop(8).Text($"Kapanış bakiyesi: {data.ClosingBalance:N2} ₺").Bold();
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
        ws.Cell("A3").Value = $"Açılış bakiyesi: {data.OpeningBalance:N2}";
        ws.Cell("A5").Value = "Tarih";
        ws.Cell("B5").Value = "Açıklama";
        ws.Cell("C5").Value = "Borç";
        ws.Cell("D5").Value = "Alacak";
        ws.Cell("E5").Value = "Bakiye";
        int row = 6;
        foreach (var r in data.Rows)
        {
            ws.Cell(row, 1).Value = r.Date.ToString("d");
            ws.Cell(row, 2).Value = r.Description;
            ws.Cell(row, 3).Value = r.Borc;
            ws.Cell(row, 4).Value = r.Alacak;
            ws.Cell(row, 5).Value = r.Bakiye;
            row++;
        }
        ws.Cell(row, 1).Value = "Kapanış bakiyesi:";
        ws.Cell(row, 5).Value = data.ClosingBalance;
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
        var period = $"Dönem: {(data.DateFrom?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")} - {(data.DateTo?.ToString("d", CultureInfo.GetCultureInfo("tr-TR")) ?? "—")}";
        var statusNames = new[] { "Taslak", "Kesildi", "Ödendi", "İptal" };

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
                            c.ConstantColumn(90);
                            c.ConstantColumn(80);
                            c.RelativeColumn();
                            c.ConstantColumn(60);
                            c.ConstantColumn(90);
                            c.ConstantColumn(40);
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
                        foreach (var row in data.Items)
                        {
                            t.Cell().Padding(4).Text(row.InvoiceNumber);
                            t.Cell().Padding(4).Text(row.InvoiceDate.ToString("d", CultureInfo.GetCultureInfo("tr-TR")));
                            t.Cell().Padding(4).Text(row.CustomerTitle);
                            t.Cell().Padding(4).Text(row.Status >= 0 && row.Status < statusNames.Length ? statusNames[row.Status] : "");
                            t.Cell().Padding(4).AlignRight().Text(row.GrandTotal.ToString("N2", CultureInfo.GetCultureInfo("tr-TR")));
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
        var statusNames = new[] { "Taslak", "Kesildi", "Ödendi", "İptal" };

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Fatura raporu");
        ws.Cell("A1").Value = data.CustomerTitle != null ? $"Fatura raporu - {data.CustomerTitle}" : "Fatura raporu";
        ws.Cell("A2").Value = $"Dönem: {(data.DateFrom?.ToString("d") ?? "—")} - {(data.DateTo?.ToString("d") ?? "—")}";
        ws.Cell("A4").Value = "Fatura No";
        ws.Cell("B4").Value = "Tarih";
        ws.Cell("C4").Value = "Cari";
        ws.Cell("D4").Value = "Durum";
        ws.Cell("E4").Value = "Toplam";
        ws.Cell("F4").Value = "Para";
        int row = 5;
        foreach (var r in data.Items)
        {
            ws.Cell(row, 1).Value = r.InvoiceNumber;
            ws.Cell(row, 2).Value = r.InvoiceDate.ToString("d");
            ws.Cell(row, 3).Value = r.CustomerTitle;
            ws.Cell(row, 4).Value = r.Status >= 0 && r.Status < statusNames.Length ? statusNames[r.Status] : "";
            ws.Cell(row, 5).Value = r.GrandTotal;
            ws.Cell(row, 6).Value = r.Currency;
            row++;
        }
        using var ms = new MemoryStream();
        wb.SaveAs(ms, false);
        return ms.ToArray();
    }
}
