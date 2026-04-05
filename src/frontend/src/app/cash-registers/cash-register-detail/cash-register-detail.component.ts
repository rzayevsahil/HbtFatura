import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CashRegisterService } from '../../services/cash-register.service';
import { CashRegisterDto, CashTransactionDto, PagedResult } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-cash-register-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
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
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private api: CashRegisterService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  get txnTypeSearchableOptions(): SearchableSelectOption[] {
    return [
      { id: '1', primary: this.translate.instant('common.txnIn') },
      { id: '2', primary: this.translate.instant('common.txnOut') }
    ];
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: c => { this.cashRegister = c; },
        error: () => { this.loading = false; }
      });
      this.loadTransactions();
    } else {
      this.loading = false;
    }
  }

  /** datetime-local çoğu tarayıcıda ss göndermez; API için tutarlı ISO parçası. */
  private normalizeDateTimeLocalForApi(s: string | undefined): string | undefined {
    const v = s?.trim();
    if (!v) return undefined;
    if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$/.test(v)) return `${v}:00`;
    return v;
  }

  loadTransactions(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loading = true;
    const dateFrom = this.normalizeDateTimeLocalForApi(this.dateFrom);
    const dateTo = this.normalizeDateTimeLocalForApi(this.dateTo);
    this.api.getTransactions(id, this.page, this.pageSize, dateFrom, dateTo).subscribe({
      next: (res: PagedResult<CashTransactionDto>) => {
        this.transactions = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
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
      description: this.newTransaction.description || this.translate.instant('cashRegisters.manualSlipNote')
    }).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('cashRegisters.toastSlipAdded'));
        this.closeAddModal();
        this.api.getById(id).subscribe(c => this.cashRegister = c);
        this.loadTransactions();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('cashRegisters.toastSlipFailed'))
    });
  }

  typeLabel(t: number): string {
    return t === 1 ? this.translate.instant('common.txnIn') : this.translate.instant('common.txnOut');
  }

  prevPage(): void { if (this.page > 1) { this.page--; this.loadTransactions(); } }
  nextPage(): void { this.page++; this.loadTransactions(); }
}
