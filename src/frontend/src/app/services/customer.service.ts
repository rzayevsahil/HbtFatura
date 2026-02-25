import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface CustomerDto {
  id: string;
  mainAccountCode?: string;
  code?: string;
  title: string;
  taxPayerType: number;
  cardType: number;
  taxNumber?: string;
  address?: string;
  city?: string;
  district?: string;
  postalCode?: string;
  country?: string;
  phone?: string;
  email?: string;
  balance: number;
}

export interface CustomerListDto extends CustomerDto {
  createdAt: string;
  totalDebit: number;
  totalCredit: number;
}

export interface AccountTransactionDto {
  id: string;
  date: string;
  description: string;
  type: number;
  amount: number;
  currency: string;
  referenceType: string;
  referenceId?: string;
  runningBalance: number;
}

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private base = '/api/customers';
  constructor(private api: ApiService) {}

  getPaged(page: number, pageSize: number, search?: string): Observable<PagedResult<CustomerListDto>> {
    return this.api.get<PagedResult<CustomerListDto>>(this.base, { page, pageSize, search: search ?? undefined });
  }

  getDropdown(): Observable<CustomerDto[]> {
    return this.api.get<CustomerDto[]>(`${this.base}/dropdown`);
  }

  getById(id: string): Observable<CustomerDto> {
    return this.api.get<CustomerDto>(`${this.base}/${id}`);
  }

  getBalance(id: string): Observable<number> {
    return this.api.get<number>(`${this.base}/${id}/balance`);
  }

  getTransactions(id: string, page: number, pageSize: number, dateFrom?: string, dateTo?: string): Observable<PagedResult<AccountTransactionDto>> {
    const params: Record<string, string | number> = { page, pageSize };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<PagedResult<AccountTransactionDto>>(`${this.base}/${id}/transactions`, params);
  }

  create(dto: Partial<CustomerDto>): Observable<CustomerDto> {
    return this.api.post<CustomerDto>(this.base, dto);
  }

  update(id: string, dto: Partial<CustomerDto>): Observable<CustomerDto> {
    return this.api.put<CustomerDto>(`${this.base}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }
}
