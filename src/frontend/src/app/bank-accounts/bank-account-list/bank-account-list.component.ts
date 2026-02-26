import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BankAccountService, BankAccountDto } from '../../services/bank-account.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-bank-account-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './bank-account-list.component.html',
  styleUrls: ['./bank-account-list.component.scss']
})
export class BankAccountListComponent implements OnInit {
  items: BankAccountDto[] = [];
  firmId: string | undefined;
  search = '';
  loading = false;

  constructor(
    private api: BankAccountService,
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

  get filteredItems(): BankAccountDto[] {
    const q = this.search?.trim().toLowerCase() || '';
    if (!q) return this.items;
    return this.items.filter(
      b =>
        b.name?.toLowerCase().includes(q) ||
        b.bankName?.toLowerCase().includes(q) ||
        b.iban?.toLowerCase().includes(q)
    );
  }

  delete(id: string, name: string): void {
    if (!confirm(`"${name}" banka hesabını silmek istediğinize emin misiniz?`)) return;
    this.api.delete(id).subscribe({
      next: () => {
        this.toastr.success('Banka hesabı silindi.');
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.')
    });
  }
}
