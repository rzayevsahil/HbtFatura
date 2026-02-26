import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomerService, CustomerListDto } from '../../services/customer.service';
import { ToastrService } from 'ngx-toastr';

import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ConfirmModalComponent],
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {
  items: CustomerListDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  search = '';
  loading = false;
  showDeleteModal = false;
  deletingCustomer: { id: string, title: string } | null = null;

  constructor(private api: CustomerService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getPaged(this.page, this.pageSize, this.search || undefined).subscribe({
      next: res => {
        this.items = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  onDeleteClick(id: string, title: string): void {
    this.deletingCustomer = { id, title };
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (!this.deletingCustomer) return;
    this.api.delete(this.deletingCustomer.id).subscribe({
      next: () => {
        this.toastr.success('Cari silindi.');
        this.showDeleteModal = false;
        this.deletingCustomer = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.');
        this.showDeleteModal = false;
        this.deletingCustomer = null;
      }
    });
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.load(); }
  }

  nextPage(): void {
    this.page++; this.load();
  }

  cardTypeLabel(t: number): string {
    return t === 2 ? 'Satıcı' : 'Alıcı';
  }
}
