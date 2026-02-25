import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CashRegisterService, CashRegisterDto, CashTransactionDto } from '../../services/cash-register.service';
import { PagedResult } from '../../core/services/api.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cash-register-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cash-register-detail.component.html',
  styleUrls: ['./cash-register-detail.component.scss']
})
export class CashRegisterDetailComponent implements OnInit {
  cashRegister: CashRegisterDto | null = null;
  transactions: CashTransactionDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  dateFrom = '';
  dateTo = '';
  showAddModal = false;
  newTransaction = { date: '', type: 1, amount: 0, description: '' };

  constructor(
    private route: ActivatedRoute,
    private api: CashRegisterService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe(c => this.cashRegister = c);
      this.loadTransactions();
    }
  }

  loadTransactions(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    const dateFrom = this.dateFrom || undefined;
    const dateTo = this.dateTo || undefined;
    this.api.getTransactions(id, this.page, this.pageSize, dateFrom, dateTo).subscribe((res: PagedResult<CashTransactionDto>) => {
      this.transactions = res.items;
      this.totalCount = res.totalCount;
    });
  }

  applyFilter(): void {
    this.page = 1;
    this.loadTransactions();
  }

  openAddModal(): void {
    this.newTransaction = { date: new Date().toISOString().slice(0, 10), type: 1, amount: 0, description: '' };
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  submitTransaction(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || this.newTransaction.amount <= 0) return;
    this.api.addTransaction(id, {
      date: this.newTransaction.date,
      type: this.newTransaction.type,
      amount: this.newTransaction.amount,
      description: this.newTransaction.description || 'Manuel fiş'
    }).subscribe({
      next: () => {
        this.toastr.success('Fiş eklendi.');
        this.closeAddModal();
        this.api.getById(id).subscribe(c => this.cashRegister = c);
        this.loadTransactions();
      },
      error: e => this.toastr.error(e.error?.message ?? 'Fiş eklenemedi.')
    });
  }

  typeLabel(t: number): string {
    return t === 1 ? 'Giriş' : 'Çıkış';
  }

  prevPage(): void { if (this.page > 1) { this.page--; this.loadTransactions(); } }
  nextPage(): void { this.page++; this.loadTransactions(); }
}
