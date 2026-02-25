import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CashRegisterService, CashRegisterDto } from '../../services/cash-register.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cash-register-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cash-register-list.component.html',
  styleUrls: ['./cash-register-list.component.scss']
})
export class CashRegisterListComponent implements OnInit {
  items: CashRegisterDto[] = [];
  firmId: string | undefined;

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
