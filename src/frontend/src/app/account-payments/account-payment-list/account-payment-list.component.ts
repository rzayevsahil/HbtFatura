import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AccountPaymentService } from '../../services/account-payment.service';
import { CustomerService } from '../../services/customer.service';
import { CustomerDto, AccountPaymentListDto } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  SearchableSelectComponent,
  SearchableSelectOption
} from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-account-payment-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './account-payment-list.component.html',
  styleUrls: ['./account-payment-list.component.scss']
})
export class AccountPaymentListComponent implements OnInit {
  listItems: AccountPaymentListDto[] = [];
  listTotalCount = 0;
  listPage = 1;
  listPageSize = 20;
  listLoading = false;
  listDateFrom = '';
  listDateTo = '';
  listCustomerId = '';
  listType = '';
  customers: CustomerDto[] = [];

  get paymentCustomerFilterOptions(): SearchableSelectOption[] {
    return this.customers.map((c) => ({
      id: c.id,
      primary: c.title,
      secondary: c.code || undefined
    }));
  }

  get paymentTypeFilterOptions(): SearchableSelectOption[] {
    return [
      { id: 'Tahsilat', primary: this.translate.instant('payments.typeCollection') },
      { id: 'Odeme', primary: this.translate.instant('payments.typePayment') }
    ];
  }

  onListCustomerFilterChange(v: string | null): void {
    this.listCustomerId = v ?? '';
    this.applyListFilter();
  }

  onListTypeFilterChange(v: string | null): void {
    this.listType = v ?? '';
    this.applyListFilter();
  }

  constructor(
    private paymentApi: AccountPaymentService,
    private customerApi: CustomerService,
    public auth: AuthService,
    private translate: TranslateService
  ) { }

  paymentTypeLabel(type: string): string {
    if (type === 'Tahsilat') return this.translate.instant('payments.typeCollection');
    if (type === 'Odeme') return this.translate.instant('payments.typePayment');
    return type;
  }

  ngOnInit(): void {
    this.customerApi.getDropdown().subscribe(c => this.customers = c);
    this.loadList();
  }

  loadList(): void {
    this.listLoading = true;
    const params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; customerId?: string; type?: string } = {
      page: this.listPage,
      pageSize: this.listPageSize
    };
    if (this.listDateFrom) params.dateFrom = this.listDateFrom;
    if (this.listDateTo) params.dateTo = this.listDateTo;
    if (this.listCustomerId) params.customerId = this.listCustomerId;
    if (this.listType) params.type = this.listType;
    this.paymentApi.getPaged(params).subscribe({
      next: res => {
        this.listItems = res.items;
        this.listTotalCount = res.totalCount;
        this.listLoading = false;
      },
      error: () => { this.listLoading = false; }
    });
  }

  applyListFilter(): void {
    this.listPage = 1;
    this.loadList();
  }

  listPrevPage(): void {
    if (this.listPage > 1) { this.listPage--; this.loadList(); }
  }

  listNextPage(): void {
    this.listPage++; this.loadList();
  }
}
