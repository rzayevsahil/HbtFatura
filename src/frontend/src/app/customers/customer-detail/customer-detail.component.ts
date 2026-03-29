import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomerService } from '../../services/customer.service';
import { CustomerDto, AccountTransactionDto, PagedResult } from '../../core/models';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule],
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.scss']
})
export class CustomerDetailComponent implements OnInit {
  customer: CustomerDto | null = null;
  loading = true;
  transactions: AccountTransactionDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  dateFrom = '';
  dateTo = '';

  constructor(private route: ActivatedRoute, private api: CustomerService, private translate: TranslateService) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: (c) => {
          this.customer = c;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
      this.loadTransactions();
    } else {
      this.loading = false;
    }
  }

  loadTransactions(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    const dateFrom = this.dateFrom || undefined;
    const dateTo = this.dateTo || undefined;
    this.api.getTransactions(id, this.page, this.pageSize, dateFrom, dateTo).subscribe((res: PagedResult<AccountTransactionDto>) => {
      this.transactions = res.items;
      this.totalCount = res.totalCount;
    });
  }

  applyFilter(): void {
    this.page = 1;
    this.loadTransactions();
  }

  taxPayerTypeLabel(t: number): string {
    return t === 1
      ? this.translate.instant('customers.taxPayerPerson')
      : this.translate.instant('customers.taxPayerCompany');
  }

  cardTypeLabel(t: number): string {
    if (t === 1) return this.translate.instant('customers.cardBuyer');
    if (t === 2) return this.translate.instant('customers.cardSeller');
    return '—';
  }

  transactionTypeLabel(t: number): string {
    return t === 1 ? 'Borç' : 'Alacak';
  }

  joinCityDistrict(city?: string | null, district?: string | null): string {
    return [district ?? '', city ?? ''].filter(s => s.length > 0).join(' / ');
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.loadTransactions(); }
  }

  nextPage(): void {
    this.page++; this.loadTransactions();
  }
}
