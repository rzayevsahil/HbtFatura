import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface ChequeOrPromissoryDto {
  id: string;
  firmId: string;
  type: number;
  customerId: string;
  customerTitle: string;
  amount: number;
  issueDate: string;
  dueDate: string;
  status: number;
  referenceType?: string;
  referenceId?: string;
  bankAccountId?: string;
  bankAccountName?: string;
  createdAt: string;
}

export interface CreateChequeOrPromissoryRequest {
  type: number;
  customerId: string;
  amount: number;
  issueDate: string;
  dueDate: string;
  referenceType?: string;
  referenceId?: string;
  bankAccountId?: string;
  firmId?: string;
}

export interface UpdateChequeOrPromissoryRequest {
  type: number;
  customerId: string;
  amount: number;
  issueDate: string;
  dueDate: string;
  referenceType?: string;
  referenceId?: string;
  bankAccountId?: string;
}

@Injectable({ providedIn: 'root' })
export class ChequeService {
  private base = '/api/cheque-or-promissories';
  constructor(private api: ApiService) {}

  getPaged(
    page: number,
    pageSize: number,
    params?: { type?: number; status?: number; customerId?: string; firmId?: string; dueFrom?: string; dueTo?: string }
  ): Observable<PagedResult<ChequeOrPromissoryDto>> {
    const p: Record<string, string | number> = { page, pageSize };
    if (params?.type != null) p['type'] = params.type;
    if (params?.status != null) p['status'] = params.status;
    if (params?.customerId) p['customerId'] = params.customerId;
    if (params?.firmId) p['firmId'] = params.firmId;
    if (params?.dueFrom) p['dueFrom'] = params.dueFrom;
    if (params?.dueTo) p['dueTo'] = params.dueTo;
    return this.api.get<PagedResult<ChequeOrPromissoryDto>>(this.base, p);
  }

  getById(id: string): Observable<ChequeOrPromissoryDto> {
    return this.api.get<ChequeOrPromissoryDto>(`${this.base}/${id}`);
  }

  create(req: CreateChequeOrPromissoryRequest): Observable<ChequeOrPromissoryDto> {
    return this.api.post<ChequeOrPromissoryDto>(this.base, req);
  }

  update(id: string, req: UpdateChequeOrPromissoryRequest): Observable<ChequeOrPromissoryDto> {
    return this.api.put<ChequeOrPromissoryDto>(`${this.base}/${id}`, req);
  }

  setStatus(id: string, status: number): Observable<void> {
    return this.api.patch<void>(`${this.base}/${id}/status`, { status });
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }
}
