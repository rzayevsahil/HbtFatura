import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export type DeliveryNoteStatus = 0 | 1 | 2 | 3; // Taslak, Onaylandi, Iptal, Faturalandi
export type InvoiceType = 0 | 1;

export interface DeliveryNoteItemDto {
  id: string;
  productId?: string;
  orderItemId?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  sortOrder: number;
}

export interface DeliveryNoteDto {
  id: string;
  deliveryNumber: string;
  customerId?: string;
  customerTitle?: string;
  orderId?: string;
  orderNumber?: string;
  invoiceId?: string | null;
  deliveryDate: string;
  /** API bazen enum'ı string ("Taslak", "Onaylandi", "Iptal") döner. */
  status: DeliveryNoteStatus | string;
  deliveryType: InvoiceType;
  createdAt: string;
  items: DeliveryNoteItemDto[];
}

export interface DeliveryNoteListDto {
  id: string;
  deliveryNumber: string;
  deliveryDate: string;
  /** API bazen enum'ı string ("Taslak", "Onaylandi", "Iptal") döner. */
  status: DeliveryNoteStatus | string;
  deliveryType: InvoiceType;
  customerTitle?: string;
  orderNumber?: string;
  invoiceId?: string | null;
}

export interface CreateDeliveryNoteFromOrderRequest {
  orderId: string;
  deliveryDate: string;
}

export interface DeliveryNoteItemInputDto {
  productId?: string | null;
  orderItemId?: string | null;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  sortOrder: number;
}

export interface CreateDeliveryNoteRequest {
  customerId?: string | null;
  orderId?: string | null;
  deliveryDate: string;
  deliveryType: InvoiceType;
  items: DeliveryNoteItemInputDto[];
}

export interface UpdateDeliveryNoteRequest {
  customerId?: string | null;
  deliveryDate: string;
  items: DeliveryNoteItemInputDto[];
}

@Injectable({ providedIn: 'root' })
export class DeliveryNoteService {
  private base = '/api/deliverynotes';
  constructor(private api: ApiService) {}

  create(req: CreateDeliveryNoteRequest): Observable<DeliveryNoteDto> {
    return this.api.post<DeliveryNoteDto>(this.base, req);
  }

  update(id: string, req: UpdateDeliveryNoteRequest): Observable<DeliveryNoteDto> {
    return this.api.put<DeliveryNoteDto>(`${this.base}/${id}`, req);
  }

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; customerId?: string; orderId?: string }): Observable<PagedResult<DeliveryNoteListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.customerId) p['customerId'] = params.customerId;
    if (params.orderId) p['orderId'] = params.orderId;
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
}
