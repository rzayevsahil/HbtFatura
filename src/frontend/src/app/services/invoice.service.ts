import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export type InvoiceStatus = 0 | 1 | 2 | 3; // Draft, Issued, Paid, Cancelled
export type InvoiceType = 0 | 1; // Satis, Alis

export interface InvoiceItemDto {
  id?: string;
  productId?: string;
  productCode?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  discountPercent: number;
  lineTotalExclVat: number;
  lineVatAmount: number;
  lineTotalInclVat: number;
  sortOrder: number;
}

export interface InvoiceItemInputDto {
  productId?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  discountPercent: number;
  sortOrder: number;
}

export interface InvoiceDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  status: InvoiceStatus;
  invoiceType: InvoiceType;
  customerId?: string;
  customerTitle: string;
  customerTaxNumber?: string;
  customerAddress?: string;
  customerPhone?: string;
  customerEmail?: string;
  subTotal: number;
  totalVat: number;
  grandTotal: number;
  currency: string;
  exchangeRate: number;
  items: InvoiceItemDto[];
  sourceType?: string | null;
  sourceId?: string | null;
  sourceNumber?: string | null;
  isGibSent?: boolean;
}

export interface InvoiceListDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  status: InvoiceStatus;
  invoiceType: InvoiceType;
  customerTitle: string;
  grandTotal: number;
  currency: string;
}

export interface CreateInvoiceRequest {
  invoiceDate: string;
  invoiceType?: InvoiceType;
  customerId?: string;
  customerTitle: string;
  customerTaxNumber?: string;
  customerAddress?: string;
  customerPhone?: string;
  customerEmail?: string;
  currency: string;
  exchangeRate: number;
  items: InvoiceItemInputDto[];
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private base = '/api/invoices';
  constructor(private api: ApiService) { }

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; invoiceType?: number; customerId?: string }): Observable<PagedResult<InvoiceListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.invoiceType !== undefined) p['invoiceType'] = params.invoiceType;
    if (params.customerId) p['customerId'] = params.customerId;
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

  sendToGib(id: string): Observable<void> {
    return this.api.post<void>(`${this.base}/${id}/send-to-gib`, {});
  }
}
