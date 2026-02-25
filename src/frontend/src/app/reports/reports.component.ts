import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService, CariExtractReportDto, CashSummaryReportDto, BankSummaryReportDto, StockLevelsReportDto } from '../services/report.service';
import { CustomerService, CustomerDto } from '../services/customer.service';
import { CashRegisterService, CashRegisterDto } from '../services/cash-register.service';
import { BankAccountService, BankAccountDto } from '../services/bank-account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

  constructor(
    private reportApi: ReportService,
    private customerApi: CustomerService,
    private cashApi: CashRegisterService,
    private bankApi: BankAccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(c => this.customers = c);
    this.cashApi.getAll().subscribe(c => this.cashRegisters = c);
    this.bankApi.getAll().subscribe(b => this.bankAccounts = b);
  }

  loadCari(): void {
    if (!this.cariCustomerId) {
      this.toastr.warning('Cari seçin.');
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
      error: e => { this.toastr.error(e.error?.message ?? 'Yüklenemedi.'); this.cariLoading = false; }
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
        this.toastr.success('PDF indirildi.');
      },
      error: e => this.toastr.error(e.error?.message ?? 'İndirilemedi.')
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
        this.toastr.success('Excel indirildi.');
      },
      error: e => this.toastr.error(e.error?.message ?? 'İndirilemedi.')
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
      error: e => { this.toastr.error(e.error?.message ?? 'Yüklenemedi.'); this.cashLoading = false; }
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
      error: e => { this.toastr.error(e.error?.message ?? 'Yüklenemedi.'); this.bankLoading = false; }
    });
  }

  loadStock(): void {
    this.stockLoading = true;
    this.reportApi.getStockLevels().subscribe({
      next: data => { this.stockData = data; this.stockLoading = false; },
      error: e => { this.toastr.error(e.error?.message ?? 'Yüklenemedi.'); this.stockLoading = false; }
    });
  }
}
