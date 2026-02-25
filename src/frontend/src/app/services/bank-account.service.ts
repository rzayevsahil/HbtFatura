import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface BankAccountDto {
  id: string;
  firmId: string;
  name: string;
  iban?: string;
  bankName?: string;
  currency: string;
  isActive: boolean;
  createdAt: string;
  balance: number;
}

export interface BankTransactionDto {
  id: string;
  date: string;
  type: number;
  amount: number;
  description: string;
  referenceType: string;
  createdAt: string;
}

export interface CreateBankAccountRequest {
  name: string;
  iban?: string;
  bankName?: string;
  currency?: string;
  firmId?: string;
}

export interface UpdateBankAccountRequest {
  name: string;
  iban?: string;
  bankName?: string;
  isActive: boolean;
}

export interface CreateBankTransactionRequest {
  date: string;
  type: number;
  amount: number;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class BankAccountService {
  private base = '/api/bankaccounts';
  constructor(private api: ApiService) {}

  getAll(firmId?: string): Observable<BankAccountDto[]> {
    return this.api.get<BankAccountDto[]>(this.base, firmId ? { firmId } : undefined);
  }

  getById(id: string): Observable<BankAccountDto> {
    return this.api.get<BankAccountDto>(`${this.base}/${id}`);
  }

  create(req: CreateBankAccountRequest): Observable<BankAccountDto> {
    return this.api.post<BankAccountDto>(this.base, req);
  }

  update(id: string, req: UpdateBankAccountRequest): Observable<BankAccountDto> {
    return this.api.put<BankAccountDto>(`${this.base}/${id}`, req);
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }

  getTransactions(id: string, page: number, pageSize: number, dateFrom?: string, dateTo?: string): Observable<PagedResult<BankTransactionDto>> {
    const params: Record<string, string | number> = { page, pageSize };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<PagedResult<BankTransactionDto>>(`${this.base}/${id}/transactions`, params);
  }

  addTransaction(id: string, req: CreateBankTransactionRequest): Observable<BankTransactionDto> {
    return this.api.post<BankTransactionDto>(`${this.base}/${id}/transactions`, req);
  }
}
