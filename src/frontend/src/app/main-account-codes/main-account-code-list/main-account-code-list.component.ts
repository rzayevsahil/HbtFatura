import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MainAccountCodeService, MainAccountCodeDto } from '../../services/main-account-code.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-main-account-code-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './main-account-code-list.component.html',
  styleUrls: ['./main-account-code-list.component.scss']
})
export class MainAccountCodeListComponent implements OnInit {
  items: MainAccountCodeDto[] = [];
  searchText = '';
  firmId: string | undefined;

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
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getByFirm(this.firmId).subscribe({
      next: list => (this.items = list),
      error: e => this.toastr.error(e.error?.message ?? 'Liste yüklenemedi.')
    });
  }

  delete(id: string, code: string): void {
    if (!confirm(`"${code}" kodunu silmek istediğinize emin misiniz?`)) return;
    this.api.delete(id).subscribe({
      next: () => {
        this.toastr.success('Ana cari kodu silindi.');
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Silme sırasında hata oluştu.')
    });
  }
}
