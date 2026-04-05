import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { OrderService } from '../../services/order.service';
import { ReportService } from '../../services/report.service';
import { OrderListDto, OrderStatus } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import {
  SearchableSelectComponent,
  SearchableSelectOption
} from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './order-list.component.html',
  styleUrls: ['./order-list.component.scss']
})
export class OrderListComponent implements OnInit {
  items: OrderListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  searchDateFrom = '';
  searchDateTo = '';
  searchStatus: OrderStatus | null = null;
  searchText = '';
  loading = false;

  constructor(
    private api: OrderService,
    private reports: ReportService,
    private router: Router,
    private toastr: ToastrService,
    public lookups: LookupService,
    public auth: AuthService,
    private translate: TranslateService
  ) { }

  get orderStatusFilterOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('OrderStatus')().map((l) => ({
      id: String(l.code),
      primary: l.name,
    }));
  }

  onOrderStatusFilterChange(v: string | null): void {
    this.searchStatus = v === null ? null : (+v as OrderStatus);
    this.page = 1;
    this.load();
  }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    const t = e.target as HTMLElement;
    if (e.key === 'F3' && !['INPUT', 'TEXTAREA', 'SELECT'].includes(t?.tagName) && !t?.closest('app-searchable-select')) {
      e.preventDefault();
      this.router.navigate(['/orders/new']);
    }
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; search?: string } = { page: this.page, pageSize: this.pageSize };
    if (this.searchDateFrom) params.dateFrom = this.searchDateFrom;
    if (this.searchDateTo) params.dateTo = this.searchDateTo;
    if (this.searchStatus !== null) params.status = this.searchStatus;
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

  statusLabel(s: OrderStatus | string | undefined): string {
    return this.lookups.getName('OrderStatus', s);
  }

  typeLabel(t: number): string {
    return this.lookups.getName('OrderType', t);
  }

  /** Toplam sütunu: API `currency` döndürür (OrderListDto); şablonda doğrudan alan erişimi NG9 tetikleyebiliyor. */
  listRowCurrency(o: OrderListDto): string {
    const raw = (o as OrderListDto & { currency?: string | null }).currency;
    const v = raw?.trim();
    return v ? v.toUpperCase() : 'TRY';
  }

  /** Bekliyor (düzenlenebilir) durumu: backend bazen sayı 0 bazen string "Bekliyor" dönebilir. */
  isEditableStatus(o: OrderListDto): boolean {
    return o.status === 0 || o.status === 'Bekliyor';
  }

  setStatus(id: string, status: OrderStatus): void {
    if (status === 2 && !confirm(this.translate.instant('common.confirmCancelOrder'))) return;
    this.api.setStatus(id, status).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.statusUpdated'));
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('common.updateFailed'))
    });
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  nextPage(): void {
    this.page++; this.load();
  }

  downloadPdf(): void {
    const dateFrom = this.searchDateFrom || undefined;
    const dateTo = this.searchDateTo || undefined;
    const status = this.searchStatus !== null ? this.searchStatus : undefined;
    const search = this.searchText || undefined;
    this.reports.downloadOrderReportPdf(dateFrom, dateTo, status, undefined, search).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `siparis-raporu-${dateStr}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }

  downloadExcel(): void {
    const dateFrom = this.searchDateFrom || undefined;
    const dateTo = this.searchDateTo || undefined;
    const status = this.searchStatus !== null ? this.searchStatus : undefined;
    const search = this.searchText || undefined;
    this.reports.downloadOrderReportExcel(dateFrom, dateTo, status, undefined, search).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `siparis-raporu-${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }
}
