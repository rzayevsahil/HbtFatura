import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface CustomerDto {
  id: string;
  title: string;
  taxNumber?: string;
  address?: string;
  phone?: string;
  email?: string;
}

export interface CustomerListDto extends CustomerDto {
  createdAt: string;
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
