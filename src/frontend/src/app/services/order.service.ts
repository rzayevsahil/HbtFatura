import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export type OrderStatus = 0 | 1 | 2; // Bekliyor, TamamiTeslim, Iptal
export type InvoiceType = 0 | 1; // Satis, Alis

export interface OrderItemDto {
  id: string;
  productId?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  sortOrder: number;
}

export interface OrderDto {
  id: string;
  orderNumber: string;
  customerId?: string;
  customerTitle?: string;
  orderDate: string;
  status: OrderStatus;
  orderType: InvoiceType;
  createdAt: string;
  items: OrderItemDto[];
}

export interface OrderListDto {
  id: string;
  orderNumber: string;
  orderDate: string;
  status: OrderStatus;
  orderType: InvoiceType;
  customerTitle?: string;
  totalAmount?: number;
}

export interface OrderItemInputDto {
  productId?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  vatRate: number;
  sortOrder: number;
}

export interface CreateOrderRequest {
  customerId?: string;
  orderDate: string;
  orderType?: InvoiceType;
  items: OrderItemInputDto[];
}

export interface UpdateOrderRequest {
  customerId?: string;
  orderDate: string;
  items: OrderItemInputDto[];
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private base = '/api/orders';
  constructor(private api: ApiService) {}

  getPaged(params: { page: number; pageSize: number; dateFrom?: string; dateTo?: string; status?: number; customerId?: string }): Observable<PagedResult<OrderListDto>> {
    const p: Record<string, string | number> = { page: params.page, pageSize: params.pageSize };
    if (params.dateFrom) p['dateFrom'] = params.dateFrom;
    if (params.dateTo) p['dateTo'] = params.dateTo;
    if (params.status !== undefined) p['status'] = params.status;
    if (params.customerId) p['customerId'] = params.customerId;
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
}
