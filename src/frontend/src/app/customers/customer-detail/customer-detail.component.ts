import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomerService, CustomerDto, AccountTransactionDto } from '../../services/customer.service';
import { PagedResult } from '../../core/services/api.service';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.scss']
})
export class CustomerDetailComponent implements OnInit {
  customer: CustomerDto | null = null;
  transactions: AccountTransactionDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 20;
  dateFrom = '';
  dateTo = '';

  constructor(private route: ActivatedRoute, private api: CustomerService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe(c => this.customer = c);
      this.loadTransactions();
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

  accountTypeLabel(t: number): string {
    return t === 2 ? 'Tedarikçi' : 'Cari';
  }

  transactionTypeLabel(t: number): string {
    return t === 1 ? 'Borç' : 'Alacak';
  }

  prevPage(): void {
    if (this.page > 1) { this.page--; this.loadTransactions(); }
  }

  nextPage(): void {
    this.page++; this.loadTransactions();
  }
}
