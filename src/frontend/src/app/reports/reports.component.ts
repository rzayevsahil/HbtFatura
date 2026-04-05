import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ReportService } from '../services/report.service';
import { CustomerService } from '../services/customer.service';
import { CashRegisterService } from '../services/cash-register.service';
import { BankAccountService } from '../services/bank-account.service';
import { ProductService } from '../services/product.service';
import {
  CariExtractReportDto, CashSummaryReportDto, BankSummaryReportDto,
  StockLevelsReportDto, InvoiceReportDto, MonthlyProductSalesReportDto,
  CustomerDto, CashRegisterDto, BankAccountDto, ProductDto
} from '../core/models';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../core/services/lookup.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  SearchableSelectComponent,
  SearchableSelectOption
} from '../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule, SearchableSelectComponent],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {
  customers: CustomerDto[] = [];
  cashRegisters: CashRegisterDto[] = [];
  bankAccounts: BankAccountDto[] = [];

  cariCustomerId = '';
  cariDateFrom = '';
  cariDateTo = '';
  cariData: CariExtractReportDto | null = null;
  cariLoading = false;

  cashRegisterId = '';
  cashDateFrom = '';
  cashDateTo = '';
  cashData: CashSummaryReportDto | null = null;
  cashLoading = false;

  bankAccountId = '';
  bankDateFrom = '';
  bankDateTo = '';
  bankData: BankSummaryReportDto | null = null;
  bankLoading = false;

  stockData: StockLevelsReportDto | null = null;
  stockLoading = false;

  invoiceDateFrom = '';
  invoiceDateTo = '';
  invoiceCustomerId = '';
  invoiceData: InvoiceReportDto | null = null;
  invoiceLoading = false;

  products: ProductDto[] = [];
  monthlyDateFrom = '';
  monthlyDateTo = '';
  monthlyProductId = '';
  monthlyData: MonthlyProductSalesReportDto | null = null;
  monthlyLoading = false;

  get reportCustomerOptions(): SearchableSelectOption[] {
    return this.customers.map((c) => ({
      id: c.id,
      primary: c.title,
      secondary: c.code || undefined
    }));
  }

  get reportCashRegisterOptions(): SearchableSelectOption[] {
    return this.cashRegisters.map((c) => ({
      id: c.id,
      primary: c.name,
      secondary: c.currency
    }));
  }

  get reportBankAccountOptions(): SearchableSelectOption[] {
    return this.bankAccounts.map((b) => ({
      id: b.id,
      primary: b.name,
      secondary: b.bankName
    }));
  }

  get reportProductOptions(): SearchableSelectOption[] {
    return this.products.map((p) => ({
      id: p.id,
      primary: p.name,
      secondary: p.code
    }));
  }

  constructor(
    private reportApi: ReportService,
    private customerApi: CustomerService,
    private cashApi: CashRegisterService,
    private bankApi: BankAccountService,
    private productApi: ProductService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(c => this.customers = c);
    this.cashApi.getAll().subscribe(c => this.cashRegisters = c);
    this.bankApi.getAll().subscribe(b => this.bankAccounts = b);
    this.productApi.getDropdown().subscribe(p => this.products = p);
  }

  loadMonthlyProductSales(): void {
    this.monthlyLoading = true;
    this.monthlyData = null;
    this.reportApi.getMonthlyProductSales(
      this.monthlyDateFrom || undefined,
      this.monthlyDateTo || undefined,
      this.monthlyProductId || undefined
    ).subscribe({
      next: data => { this.monthlyData = data; this.monthlyLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.monthlyLoading = false; }
    });
  }

  monthName(month: number): string {
    const names = ['Ocak', 'Şubat', 'Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos', 'Eylül', 'Ekim', 'Kasım', 'Aralık'];
    return month >= 1 && month <= 12 ? names[month - 1] : '';
  }

  loadCari(): void {
    if (!this.cariCustomerId) {
      this.toastr.warning(this.translate.instant('reportsPage.selectCustomerWarn'));
      return;
    }
    this.cariLoading = true;
    this.cariData = null;
    this.reportApi.getCariExtract(
      this.cariCustomerId,
      this.cariDateFrom || undefined,
      this.cariDateTo || undefined
    ).subscribe({
      next: data => { this.cariData = data; this.cariLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.cariLoading = false; }
    });
  }

  downloadCariPdf(): void {
    if (!this.cariCustomerId) return;
    this.reportApi.downloadCariExtractPdf(
      this.cariCustomerId,
      this.cariDateFrom || undefined,
      this.cariDateTo || undefined
    ).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `cari-ekstre-${this.cariCustomerId}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('reportsPage.downloadPdfOk'));
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.downloadPdfFail'))
    });
  }

  downloadCariExcel(): void {
    if (!this.cariCustomerId) return;
    this.reportApi.downloadCariExtractExcel(
      this.cariCustomerId,
      this.cariDateFrom || undefined,
      this.cariDateTo || undefined
    ).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `cari-ekstre-${this.cariCustomerId}.xlsx`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('reportsPage.downloadExcelOk'));
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.downloadExcelFail'))
    });
  }

  loadCash(): void {
    this.cashLoading = true;
    this.cashData = null;
    this.reportApi.getCashSummary(
      this.cashRegisterId || undefined,
      this.cashDateFrom || undefined,
      this.cashDateTo || undefined
    ).subscribe({
      next: data => { this.cashData = data; this.cashLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.cashLoading = false; }
    });
  }

  loadBank(): void {
    this.bankLoading = true;
    this.bankData = null;
    this.reportApi.getBankSummary(
      this.bankAccountId || undefined,
      this.bankDateFrom || undefined,
      this.bankDateTo || undefined
    ).subscribe({
      next: data => { this.bankData = data; this.bankLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.bankLoading = false; }
    });
  }

  loadStock(): void {
    this.stockLoading = true;
    this.reportApi.getStockLevels().subscribe({
      next: data => { this.stockData = data; this.stockLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.stockLoading = false; }
    });
  }

  downloadStockPdf(): void {
    this.reportApi.downloadStockLevelsPdf().subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'stok-raporu.pdf';
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('reportsPage.downloadPdfOk'));
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.downloadPdfFail'))
    });
  }

  loadInvoiceReport(): void {
    this.invoiceLoading = true;
    this.invoiceData = null;
    this.reportApi.getInvoiceReport(
      this.invoiceDateFrom || undefined,
      this.invoiceDateTo || undefined,
      this.invoiceCustomerId || undefined
    ).subscribe({
      next: data => { this.invoiceData = data; this.invoiceLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.genericLoadFailed')); this.invoiceLoading = false; }
    });
  }

  downloadInvoiceReportPdf(): void {
    this.reportApi.downloadInvoiceReportPdf(
      this.invoiceDateFrom || undefined,
      this.invoiceDateTo || undefined,
      this.invoiceCustomerId || undefined
    ).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'fatura-raporu.pdf';
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('reportsPage.downloadPdfOk'));
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.downloadPdfFail'))
    });
  }

  downloadInvoiceReportExcel(): void {
    this.reportApi.downloadInvoiceReportExcel(
      this.invoiceDateFrom || undefined,
      this.invoiceDateTo || undefined,
      this.invoiceCustomerId || undefined
    ).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'fatura-raporu.xlsx';
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('reportsPage.downloadExcelOk'));
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('reportsPage.downloadExcelFail'))
    });
  }

  statusLabel(s: number): string {
    return this.lookups.getName('InvoiceStatus', s);
  }
}
