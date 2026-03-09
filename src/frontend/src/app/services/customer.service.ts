import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import { PagedResult, CustomerDto, CustomerListDto, AccountTransactionDto } from '../core/models';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private base = '/api/customers';
  constructor(private api: ApiService) { }

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
