import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { InvoiceService } from '../../services/invoice.service';
import { ReportService } from '../../services/report.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import { InvoiceListDto, InvoiceStatus } from '../../core/models';
import {
  SearchableSelectComponent,
  SearchableSelectOption
} from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss']
})
export class InvoiceListComponent implements OnInit {
  items: InvoiceListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  searchDateFrom = '';
  searchDateTo = '';
  searchStatus: InvoiceStatus | null = null;
  searchInvoiceType: number | null = null;
  searchText = '';
  loading = false;

  constructor(
    private api: InvoiceService,
    private reports: ReportService,
    private router: Router,
    private toastr: ToastrService,
    public lookups: LookupService,
    public auth: AuthService,
    private translate: TranslateService
  ) { }

  get invoiceStatusFilterOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('InvoiceStatus')().map((l) => ({
      id: String(l.code),
      primary: l.name
    }));
  }

  get invoiceTypeFilterOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('InvoiceType')().map((l) => ({
      id: String(l.code),
      primary: l.name
    }));
  }

  onInvoiceStatusFilterChange(v: string | null): void {
    this.searchStatus = v === null ? null : (+v as InvoiceStatus);
    this.page = 1;
    this.load();
  }

  onInvoiceTypeFilterChange(v: string | null): void {
    this.searchInvoiceType = v === null ? null : +v;
    this.page = 1;
    this.load();
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const t = e.target as HTMLElement;
    if (e.key === 'F3' && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
      e.preventDefault();
      this.router.navigate(['/invoices/new']);
    }
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const params: any = { page: this.page, pageSize: this.pageSize };
    if (this.searchDateFrom) params.dateFrom = this.searchDateFrom;
    if (this.searchDateTo) params.dateTo = this.searchDateTo;
    if (this.searchStatus !== null) params.status = this.searchStatus;
    if (this.searchInvoiceType !== null) params.invoiceType = this.searchInvoiceType;
    if (this.searchText) params.search = this.searchText;
    this.api.getPaged(params).subscribe({
      next: res => {
        this.items = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  statusLabel(s: any, sourceType?: string | null): string {
    return this.lookups.getName('InvoiceStatus', s);
  }

  statusClass(s: any): string {
    const map: any = {
      0: 'draft', 'Draft': 'draft',
      1: 'issued', 'Issued': 'issued',
      2: 'paid', 'Paid': 'paid',
      3: 'cancelled', 'Cancelled': 'cancelled'
    };
    return map[s] ?? '';
  }

  typeLabel(t: number): string {
    return this.lookups.getName('InvoiceType', t);
  }

  /** Liste satırı: şablonda doğrudan `inv.currency` NG9 tetikleyebiliyor. */
  listRowCurrency(inv: InvoiceListDto): string {
    const raw = (inv as InvoiceListDto & { currency?: string | null }).currency;
    const v = raw?.trim();
    return v ? v.toUpperCase() : 'TRY';
  }

  downloadPdf(id: string, invoiceNumber: string): void {
    this.api.getPdf(id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `fatura-${invoiceNumber}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success('Fatura PDF indirildi.');
      },
      error: () => this.toastr.error('PDF indirilemedi.')
    });
  }

  downloadReportPdf(): void {
    const dateFrom = this.searchDateFrom || undefined;
    const dateTo = this.searchDateTo || undefined;
    this.reports.downloadInvoiceReportPdf(dateFrom, dateTo).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `fatura-raporu-${dateStr}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('invoices.toastrReportPdfOk'));
      },
      error: () => this.toastr.error(this.translate.instant('invoices.toastrReportPdfFail'))
    });
  }

  downloadReportExcel(): void {
    const dateFrom = this.searchDateFrom || undefined;
    const dateTo = this.searchDateTo || undefined;
    this.reports.downloadInvoiceReportExcel(dateFrom, dateTo).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `fatura-raporu-${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.toastr.success(this.translate.instant('invoices.toastrReportExcelOk'));
      },
      error: () => this.toastr.error(this.translate.instant('invoices.toastrReportExcelFail'))
    });
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  nextPage(): void {
    this.page++; this.load();
  }
}
