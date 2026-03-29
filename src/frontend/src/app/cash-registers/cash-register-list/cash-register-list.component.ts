import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CashRegisterService } from '../../services/cash-register.service';
import { CashRegisterDto } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';
import { currencyDisplaySuffix } from '../../core/utils/currency-display';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-cash-register-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ConfirmModalComponent, TranslateModule],
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
    private toastr: ToastrService,
    private translate: TranslateService
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
        this.toastr.error(e.error?.message ?? this.translate.instant('cashRegisters.toastLoadFailed'));
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

  /** Bakiye yanında gösterilecek para kodu (core/utils/currency-display). */
  formatCurrencyLabel(code: string | undefined | null): string {
    return currencyDisplaySuffix(code);
  }

  confirmDelete(): void {
    if (!this.deletingRegister) return;
    this.api.delete(this.deletingRegister.id).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('cashRegisters.toastRegisterDeleted'));
        this.showDeleteModal = false;
        this.deletingRegister = null;
        this.load();
      },
      error: e => {
        this.toastr.error(e.error?.message ?? this.translate.instant('mainAccountCodes.deleteError'));
        this.showDeleteModal = false;
        this.deletingRegister = null;
      }
    });
  }
}
