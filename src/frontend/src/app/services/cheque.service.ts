import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import {
  PagedResult, ChequeOrPromissoryDto, CreateChequeOrPromissoryRequest,
  UpdateChequeOrPromissoryRequest
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class ChequeService {
  private base = '/api/cheque-or-promissories';
  constructor(private api: ApiService) { }

  getPaged(
    page: number,
    pageSize: number,
    params?: { type?: number; status?: number; customerId?: string; firmId?: string; dueFrom?: string; dueTo?: string; portfolioNumber?: string; serialNumber?: string }
  ): Observable<PagedResult<ChequeOrPromissoryDto>> {
    const p: Record<string, string | number> = { page, pageSize };
    if (params?.type != null) p['type'] = params.type;
    if (params?.status != null) p['status'] = params.status;
    if (params?.customerId) p['customerId'] = params.customerId;
    if (params?.firmId) p['firmId'] = params.firmId;
    if (params?.dueFrom) p['dueFrom'] = params.dueFrom;
    if (params?.dueTo) p['dueTo'] = params.dueTo;
    if (params?.portfolioNumber) p['portfolioNumber'] = params.portfolioNumber;
    if (params?.serialNumber) p['serialNumber'] = params.serialNumber;
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
