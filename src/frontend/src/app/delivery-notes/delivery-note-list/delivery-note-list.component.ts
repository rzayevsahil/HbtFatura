import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DeliveryNoteService, DeliveryNoteListDto, DeliveryNoteStatus } from '../../services/delivery-note.service';
import { ToastrService } from 'ngx-toastr';

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

  constructor(private api: DeliveryNoteService, private toastr: ToastrService) {}

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

  statusLabel(s: DeliveryNoteStatus): string {
    const map: Record<DeliveryNoteStatus, string> = { 0: 'Taslak', 1: 'Onaylandı', 2: 'İptal' };
    return map[s] ?? '';
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Alış' : 'Satış';
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
