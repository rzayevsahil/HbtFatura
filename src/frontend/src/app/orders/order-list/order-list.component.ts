import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../services/order.service';
import { OrderListDto, OrderStatus } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import { EmployeeService } from '../../services/employee.service';

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
  searchText = '';
  loading = false;
  usersMap: Record<string, string> = {};

  constructor(
    private api: OrderService,
    private router: Router,
    private toastr: ToastrService,
    public lookups: LookupService,
    private userService: EmployeeService,
    public auth: AuthService
  ) { }

  @HostListener('document:keydown', ['$event'])
  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'F3' && !['INPUT', 'TEXTAREA', 'SELECT'].includes((e.target as HTMLElement)?.tagName)) {
      e.preventDefault();
      this.router.navigate(['/orders/new']);
    }
  }

  ngOnInit(): void {
    this.loadUsers();
    this.load();
  }

  loadUsers(): void {
    this.userService.getAll().subscribe(list => {
      this.usersMap = list.reduce((acc, u) => ({ ...acc, [u.id]: u.fullName }), {});
    });
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

  getUserName(id: string): string {
    return this.usersMap[id] ?? '—';
  }

  typeLabel(t: number): string {
    return this.lookups.getName('OrderType', t);
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
