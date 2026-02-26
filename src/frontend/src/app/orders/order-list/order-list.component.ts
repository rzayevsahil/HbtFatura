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

  constructor(private api: OrderService, private router: Router, private toastr: ToastrService) {}

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
    const params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number } = { page: this.page, pageSize: this.pageSize };
    if (this.searchDateFrom) params.dateFrom = this.searchDateFrom;
    if (this.searchDateTo) params.dateTo = this.searchDateTo;
    if (this.searchStatus !== null) params.status = this.searchStatus;
    this.api.getPaged(params).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  statusLabel(s: OrderStatus): string {
    const map: Record<OrderStatus, string> = { 0: 'Bekliyor', 1: 'Tamamı Teslim', 2: 'İptal' };
    return map[s] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
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
