import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CashRegisterService, CashRegisterDto } from '../../services/cash-register.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-cash-register-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ConfirmModalComponent],
  templateUrl: './cash-register-list.component.html',
  styleUrls: ['./cash-register-list.component.scss']
})
export class CashRegisterListComponent implements OnInit {
  items: CashRegisterDto[] = [];
  firmId: string | undefined;
  search = '';
  loading = false;
  showDeleteModal = false;
  deletingRegister: { id: string, name: string } | null = null;

  constructor(
    private api: CashRegisterService,
    public auth: AuthService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getAll(this.firmId).subscribe({
      next: list => {
        this.items = list;
        this.loading = false;
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Liste yüklenemedi.');
        this.loading = false;
      }
    });
  }

  get filteredItems(): CashRegisterDto[] {
    const q = this.search?.trim().toLowerCase() || '';
    if (!q) return this.items;
    return this.items.filter(
      c =>
        c.name?.toLowerCase().includes(q) ||
        c.currency?.toLowerCase().includes(q)
    );
  }

  onDeleteClick(id: string, name: string): void {
    this.deletingRegister = { id, name };
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (!this.deletingRegister) return;
    this.api.delete(this.deletingRegister.id).subscribe({
      next: () => {
        this.toastr.success('Kasa silindi.');
        this.showDeleteModal = false;
        this.deletingRegister = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.');
        this.showDeleteModal = false;
        this.deletingRegister = null;
      }
    });
  }
}
