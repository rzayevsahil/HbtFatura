import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BankAccountService } from '../../services/bank-account.service';
import { BankAccountDto, BankTransactionDto, PagedResult } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-bank-account-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './bank-account-detail.component.html',
  styleUrls: ['./bank-account-detail.component.scss']
})
export class BankAccountDetailComponent implements OnInit {
  bankAccount: BankAccountDto | null = null;
  transactions: BankTransactionDto[] = [];
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
    private api: BankAccountService,
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
        next: b => { this.bankAccount = b; },
        error: () => { this.loading = false; }
      });
      this.loadTransactions();
    } else {
      this.loading = false;
    }
  }

  loadTransactions(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loading = true;
    this.api.getTransactions(id, this.page, this.pageSize, this.dateFrom || undefined, this.dateTo || undefined).subscribe({
      next: (res: PagedResult<BankTransactionDto>) => {
        this.transactions = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  applyFilter(): void { this.page = 1; this.loadTransactions(); }
  openAddModal(): void {
    this.newTransaction = { date: new Date().toISOString().slice(0, 10), type: 1, amount: 0, description: '' };
    this.showAddModal = true;
  }
  closeAddModal(): void { this.showAddModal = false; }
  submitTransaction(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id || this.newTransaction.amount <= 0) return;
    this.api.addTransaction(id, {
      date: this.newTransaction.date,
      type: this.newTransaction.type,
      amount: this.newTransaction.amount,
      description: this.newTransaction.description || this.translate.instant('bankAccountsPage.manualReceiptNote')
    }).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('bankAccountsPage.toastReceiptAdded'));
        this.closeAddModal();
        this.api.getById(id).subscribe(b => this.bankAccount = b);
        this.loadTransactions();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('bankAccountsPage.toastReceiptFailed'))
    });
  }
  typeLabel(t: number): string {
    return t === 1 ? this.translate.instant('common.txnIn') : this.translate.instant('common.txnOut');
  }
  prevPage(): void { if (this.page > 1) { this.page--; this.loadTransactions(); } }
  nextPage(): void { this.page++; this.loadTransactions(); }
}
