import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { InvoiceService } from '../../services/invoice.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import { InvoiceListDto, InvoiceStatus } from '../../core/models';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
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
    private router: Router,
    private toastr: ToastrService,
    public lookups: LookupService,
    public auth: AuthService
  ) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F3' && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
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

  downloadPdf(id: string): void {
    this.api.getPdf(id).subscribe({
      next: blob => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `fatura-${id}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
        this.toastr.success('Fatura PDF indirildi.');
      },
      error: () => this.toastr.error('PDF indirilemedi.')
    });
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  nextPage(): void {
    this.page++; this.load();
  }
}
