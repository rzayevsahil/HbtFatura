import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import {
  PagedResult, CashRegisterDto, CashTransactionDto,
  CreateCashRegisterRequest, UpdateCashRegisterRequest,
  CreateCashTransactionRequest
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class CashRegisterService {
  private base = '/api/cashregisters';
  constructor(private api: ApiService) { }

  getAll(firmId?: string): Observable<CashRegisterDto[]> {
    return this.api.get<CashRegisterDto[]>(this.base, firmId ? { firmId } : undefined);
  }

  getById(id: string): Observable<CashRegisterDto> {
    return this.api.get<CashRegisterDto>(`${this.base}/${id}`);
  }

  create(req: CreateCashRegisterRequest): Observable<CashRegisterDto> {
    return this.api.post<CashRegisterDto>(this.base, req);
  }

  update(id: string, req: UpdateCashRegisterRequest): Observable<CashRegisterDto> {
    return this.api.put<CashRegisterDto>(`${this.base}/${id}`, req);
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }

  getTransactions(id: string, page: number, pageSize: number, dateFrom?: string, dateTo?: string): Observable<PagedResult<CashTransactionDto>> {
    const params: Record<string, string | number> = { page, pageSize };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<PagedResult<CashTransactionDto>>(`${this.base}/${id}/transactions`, params);
  }

  addTransaction(id: string, req: CreateCashTransactionRequest): Observable<CashTransactionDto> {
    return this.api.post<CashTransactionDto>(`${this.base}/${id}/transactions`, req);
  }
}
