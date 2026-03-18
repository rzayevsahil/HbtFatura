import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeliveryNoteService } from '../../services/delivery-note.service';
import { ReportService } from '../../services/report.service';
import { DeliveryNoteListDto, DeliveryNoteStatus } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';

@Component({
  selector: 'app-delivery-note-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './delivery-note-list.component.html',
  styleUrls: ['./delivery-note-list.component.scss']
})
export class DeliveryNoteListComponent implements OnInit {
  items: DeliveryNoteListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  searchDateFrom = '';
  searchDateTo = '';
  searchStatus: DeliveryNoteStatus | null = null;
  searchText = '';
  loading = false;

  constructor(
    private api: DeliveryNoteService,
    private reports: ReportService,
    private toastr: ToastrService,
    public lookups: LookupService,
    public auth: AuthService
  ) { }

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

  /** Backend bazen enum'ı sayı bazen string (örn. "Taslak") gönderebilir. */
  statusLabel(s: DeliveryNoteStatus | string | undefined): string {
    return this.lookups.getName('DeliveryNoteStatus', s);
  }

  typeLabel(t: number): string {
    return this.lookups.getName('DeliveryNoteType', t);
  }

  /** Taslak ve faturaya aktarılmamışsa düzenlenebilir. */
  isEditable(d: DeliveryNoteListDto): boolean {
    const taslak = d.status === 0 || d.status === 'Taslak';
    return !!taslak && !d.invoiceId;
  }

  setStatus(id: string, status: DeliveryNoteStatus): void {
    this.api.setStatus(id, status).subscribe({
      next: () => { this.toastr.success('Durum güncellendi.'); this.load(); },
      error: e => this.toastr.error(e.error?.message ?? 'Güncellenemedi.')
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
    this.reports.downloadDeliveryNoteReportPdf(dateFrom, dateTo, status, undefined, search).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `irsaliye-raporu-${dateStr}.pdf`;
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
    this.reports.downloadDeliveryNoteReportExcel(dateFrom, dateTo, status, undefined, search).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        const today = new Date();
        const dateStr = today.toISOString().slice(0, 10);
        a.href = url;
        a.download = `irsaliye-raporu-${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }
}
