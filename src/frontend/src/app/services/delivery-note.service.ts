import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import {
  PagedResult, DeliveryNoteStatus, InvoiceType,
  DeliveryNoteItemDto, DeliveryNoteDto, DeliveryNoteListDto,
  CreateDeliveryNoteFromOrderRequest, DeliveryNoteItemInputDto,
  CreateDeliveryNoteRequest, UpdateDeliveryNoteRequest
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class DeliveryNoteService {
  private base = '/api/deliverynotes';
  constructor(private api: ApiService) { }

  create(req: CreateDeliveryNoteRequest): Observable<DeliveryNoteDto> {
    return this.api.post<DeliveryNoteDto>(this.base, req);
  }

  update(id: string, req: UpdateDeliveryNoteRequest): Observable<DeliveryNoteDto> {
    return this.api.put<DeliveryNoteDto>(`${this.base}/${id}`, req);
  }

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; customerId?: string; orderId?: string; search?: string }): Observable<PagedResult<DeliveryNoteListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.customerId) p['customerId'] = params.customerId;
    if (params.orderId) p['orderId'] = params.orderId;
    if (params.search) p['search'] = params.search;
    return this.api.get<PagedResult<DeliveryNoteListDto>>(this.base, p);
  }

  getById(id: string): Observable<DeliveryNoteDto> {
    return this.api.get<DeliveryNoteDto>(`${this.base}/${id}`);
  }

  createFromOrder(req: CreateDeliveryNoteFromOrderRequest): Observable<DeliveryNoteDto> {
    return this.api.post<DeliveryNoteDto>(`${this.base}/from-order`, req);
  }

  setStatus(id: string, status: DeliveryNoteStatus): Observable<void> {
    return this.api.patch<void>(`${this.base}/${id}/status`, { status });
  }

  downloadPdf(id: string): Observable<Blob> {
    return this.api.getBlob(`${this.base}/${id}/pdf`);
  }
}
