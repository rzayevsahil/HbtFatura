import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChequeService, ChequeOrPromissoryDto } from '../../services/cheque.service';
import { CustomerService } from '../../services/customer.service';
import { ToastrService } from 'ngx-toastr';
import { PagedResult } from '../../core/services/api.service';

@Component({
  selector: 'app-cheque-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cheque-list.component.html',
  styleUrls: ['./cheque-list.component.scss']
})
export class ChequeListComponent implements OnInit {
  items: ChequeOrPromissoryDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  filterType: number | '' = '';
  filterStatus: number | '' = '';
  filterDueFrom = '';
  filterDueTo = '';

  constructor(
    private api: ChequeService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getPaged(this.page, this.pageSize, {
      type: this.filterType === '' ? undefined : this.filterType,
      status: this.filterStatus === '' ? undefined : this.filterStatus,
      dueFrom: this.filterDueFrom || undefined,
      dueTo: this.filterDueTo || undefined
    }).subscribe({
      next: (res: PagedResult<ChequeOrPromissoryDto>) => {
        this.items = res.items;
        this.totalCount = res.totalCount;
      },
      error: e => this.toastr.error(e.error?.message ?? 'Liste yüklenemedi.')
    });
  }

  applyFilter(): void {
    this.page = 1;
    this.load();
  }

  delete(id: string): void {
    if (!confirm('Bu kaydı silmek istediğinize emin misiniz?')) return;
    this.api.delete(id).subscribe({
      next: () => {
        this.toastr.success('Kayıt silindi.');
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Silinemedi.')
    });
  }

  typeLabel(type: number): string {
    return type === 1 ? 'Çek' : type === 2 ? 'Senet' : '';
  }

  statusLabel(status: number): string {
    const map: Record<number, string> = { 0: 'Portföyde', 1: 'Tahsil edildi', 2: 'Ödendi', 3: 'Reddedildi' };
    return map[status] ?? '';
  }
}
