import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CashRegisterService, CashRegisterDto } from '../../services/cash-register.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cash-register-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cash-register-list.component.html',
  styleUrls: ['./cash-register-list.component.scss']
})
export class CashRegisterListComponent implements OnInit {
  items: CashRegisterDto[] = [];
  firmId: string | undefined;
  search = '';

  constructor(
    private api: CashRegisterService,
    public auth: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getAll(this.firmId).subscribe({
      next: list => (this.items = list),
      error: e => this.toastr.error(e.error?.message ?? 'Liste yüklenemedi.')
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

  delete(id: string, name: string): void {
    if (!confirm(`"${name}" kasasını silmek istediğinize emin misiniz?`)) return;
    this.api.delete(id).subscribe({
      next: () => {
        this.toastr.success('Kasa silindi.');
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.')
    });
  }
}
