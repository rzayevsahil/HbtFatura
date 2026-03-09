import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import {
  PagedResult, InvoiceStatus, InvoiceType, InvoiceScenario,
  InvoiceItemDto, InvoiceItemInputDto, InvoiceDto,
  InvoiceListDto, CreateInvoiceRequest
} from '../core/models';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private base = '/api/invoices';
  constructor(private api: ApiService) { }

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; invoiceType?: number; customerId?: string; search?: string }): Observable<PagedResult<InvoiceListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.invoiceType !== undefined) p['invoiceType'] = params.invoiceType;
    if (params.customerId) p['customerId'] = params.customerId;
    if (params.search) p['search'] = params.search;
    return this.api.get<PagedResult<InvoiceListDto>>(this.base, p);
  }

  getById(id: string): Observable<InvoiceDto> {
    return this.api.get<InvoiceDto>(`${this.base}/${id}`);
  }

  create(req: CreateInvoiceRequest): Observable<InvoiceDto> {
    return this.api.post<InvoiceDto>(this.base, req);
  }

  createFromDeliveryNote(deliveryNoteId: string): Observable<InvoiceDto> {
    return this.api.post<InvoiceDto>(`${this.base}/from-delivery-note`, { deliveryNoteId });
  }

  update(id: string, req: CreateInvoiceRequest, rowVersion?: string): Observable<InvoiceDto> {
    const headers = rowVersion ? { 'If-Match': rowVersion } : {};
    return this.api.put<InvoiceDto>(`${this.base}/${id}`, req);
  }

  setStatus(id: string, status: InvoiceStatus): Observable<void> {
    return this.api.patch<void>(`${this.base}/${id}/status`, { status });
  }

  getPdf(id: string): Observable<Blob> {
    return this.api.getBlob(`${this.base}/${id}/pdf`);
  }

  sendToGib(id: string, scenario: InvoiceScenario): Observable<void> {
    return this.api.post<void>(`${this.base}/${id}/send-to-gib`, { scenario });
  }
}
