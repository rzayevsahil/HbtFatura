import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import {
  PagedResult, OrderStatus, InvoiceType, OrderItemDto,
  OrderDto, OrderListDto, OrderItemInputDto,
  CreateOrderRequest, UpdateOrderRequest
} from '../core/models';
@Injectable({ providedIn: 'root' })
export class OrderService {
  private base = '/api/orders';
  constructor(private api: ApiService) { }

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; customerId?: string; search?: string }): Observable<PagedResult<OrderListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.customerId) p['customerId'] = params.customerId;
    if (params.search) p['search'] = params.search;
    return this.api.get<PagedResult<OrderListDto>>(this.base, p);
  }

  getById(id: string): Observable<OrderDto> {
    return this.api.get<OrderDto>(`${this.base}/${id}`);
  }

  create(req: CreateOrderRequest): Observable<OrderDto> {
    return this.api.post<OrderDto>(this.base, req);
  }

  update(id: string, req: UpdateOrderRequest): Observable<OrderDto> {
    return this.api.put<OrderDto>(`${this.base}/${id}`, req);
  }

  setStatus(id: string, status: OrderStatus): Observable<void> {
    return this.api.patch<void>(`${this.base}/${id}/status`, { status });
  }

  downloadPdf(id: string): Observable<Blob> {
    return this.api.getBlob(`${this.base}/${id}/pdf`);
  }
}
