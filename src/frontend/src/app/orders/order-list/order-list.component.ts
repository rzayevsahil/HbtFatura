import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { OrderService, OrderListDto, OrderStatus } from '../../services/order.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
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
  loading = false;

  constructor(private api: OrderService, private router: Router, private toastr: ToastrService) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F3' && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
      e.preventDefault();
      this.router.navigate(['/orders/new']);
    }
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    const params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number } = { page: this.page, pageSize: this.pageSize };
    if (this.searchDateFrom) params.dateFrom = this.searchDateFrom;
    if (this.searchDateTo) params.dateTo = this.searchDateTo;
    if (this.searchStatus !== null) params.status = this.searchStatus;
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
    if (s === undefined || s === null) return '';
    const mapByNumber: Record<number, string> = { 0: 'Bekliyor', 1: 'Tamamı Teslim', 2: 'İptal', 3: 'Onaylandı', 4: 'Kısmi Teslim' };
    const mapByString: Record<string, string> = { Bekliyor: 'Bekliyor', TamamiTeslim: 'Tamamı Teslim', Iptal: 'İptal', Onaylandi: 'Onaylandı', KismiTeslim: 'Kısmi Teslim' };
    if (typeof s === 'number') return mapByNumber[s] ?? '';
    return mapByString[String(s)] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
  }

  /** Bekliyor (düzenlenebilir) durumu: backend bazen sayı 0 bazen string "Bekliyor" dönebilir. */
  isEditableStatus(o: OrderListDto): boolean {
    return o.status === 0 || o.status === 'Bekliyor';
  }

  setStatus(id: string, status: OrderStatus): void {
    if (status === 2 && !confirm('Siparişi iptal etmek istediğinize emin misiniz?')) return;
    this.api.setStatus(id, status).subscribe({
      next: () => {
        this.toastr.success('Durum güncellendi.');
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Güncellenemedi.')
    });
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  nextPage(): void {
    this.page++; this.load();
  }
}
