import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeliveryNoteService, DeliveryNoteListDto, DeliveryNoteStatus } from '../../services/delivery-note.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../core/services/auth.service';
import { LookupService } from '../../core/services/lookup.service';
import { EmployeeService } from '../../services/employee.service';

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
  usersMap: Record<string, string> = {};

  constructor(
    private api: DeliveryNoteService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private userService: EmployeeService,
    public auth: AuthService
  ) { }

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

  /** Backend bazen enum'ı sayı bazen string (örn. "Taslak") gönderebilir. */
  statusLabel(s: DeliveryNoteStatus | string | undefined): string {
    return this.lookups.getName('DeliveryNoteStatus', s);
  }

  getUserName(id: string): string {
    return this.usersMap[id] ?? '—';
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
}
