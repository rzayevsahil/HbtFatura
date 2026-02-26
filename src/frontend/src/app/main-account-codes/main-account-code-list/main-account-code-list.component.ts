import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MainAccountCodeService, MainAccountCodeDto } from '../../services/main-account-code.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { MainAccountCodeFormComponent } from '../main-account-code-form/main-account-code-form.component';

import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-main-account-code-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, MainAccountCodeFormComponent, ConfirmModalComponent],
  templateUrl: './main-account-code-list.component.html',
  styleUrls: ['./main-account-code-list.component.scss']
})
export class MainAccountCodeListComponent implements OnInit {
  items: MainAccountCodeDto[] = [];
  searchText = '';
  firmId: string | undefined;
  modalOpen = false;
  editingId: string | null = null;
  loading = false;
  showDeleteModal = false;
  deletingItem: { id: string, code: string } | null = null;

  get filteredItems(): MainAccountCodeDto[] {
    const q = this.searchText.trim().toLowerCase();
    if (!q) return this.items;
    return this.items.filter(
      x =>
        (x.code ?? '').toLowerCase().includes(q) ||
        (x.name ?? '').toLowerCase().includes(q)
    );
  }

  constructor(
    private api: MainAccountCodeService,
    public auth: AuthService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getByFirm(this.firmId).subscribe({
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

  onDeleteClick(id: string, code: string): void {
    this.deletingItem = { id, code };
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (!this.deletingItem) return;
    this.api.delete(this.deletingItem.id).subscribe({
      next: () => {
        this.toastr.success('Ana cari kodu silindi.');
        this.showDeleteModal = false;
        this.deletingItem = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.');
        this.showDeleteModal = false;
        this.deletingItem = null;
      }
    });
  }

  openNew(): void {
    this.editingId = null;
    this.modalOpen = true;
  }

  openEdit(id: string): void {
    this.editingId = id;
    this.modalOpen = true;
  }

  closeModal(): void {
    this.modalOpen = false;
    this.editingId = null;
  }

  onFormSaved(): void {
    this.closeModal();
    this.load();
  }
}
