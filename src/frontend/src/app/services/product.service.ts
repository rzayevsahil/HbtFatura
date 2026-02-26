import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService, PagedResult } from '../core/services/api.service';

export interface ProductDto {
  id: string;
  firmId: string;
  code: string;
  name: string;
  barcode?: string;
  unit: string;
  minStock: number;
  maxStock: number;
  stockQuantity: number;
  createdAt: string;
}

export interface ProductListDto extends ProductDto { }

export interface CreateProductRequest {
  code: string;
  name: string;
  barcode?: string;
  unit?: string;
  minStock?: number;
  maxStock?: number;
  stockQuantity?: number;
  firmId?: string;
}

export interface UpdateProductRequest {
  code: string;
  name: string;
  barcode?: string;
  unit?: string;
  minStock?: number;
  maxStock?: number;
  stockQuantity?: number;
}

export interface StockMovementDto {
  id: string;
  date: string;
  type: number;
  quantity: number;
  referenceType: string;
  referenceId?: string;
  createdAt: string;
}

export interface CreateStockMovementRequest {
  date: string;
  type: number;
  quantity: number;
  description: string;
}

@Injectable({ providedIn: 'root' })
export class ProductService {
  private base = '/api/products';
  constructor(private api: ApiService) { }

  getPaged(page: number, pageSize: number, search?: string, firmId?: string): Observable<PagedResult<ProductListDto>> {
    const params: Record<string, string | number> = { page, pageSize };
    if (search) params['search'] = search;
    if (firmId) params['firmId'] = firmId;
    return this.api.get<PagedResult<ProductListDto>>(this.base, params);
  }

  getDropdown(firmId?: string): Observable<ProductDto[]> {
    return this.api.get<ProductDto[]>(`${this.base}/dropdown`, firmId ? { firmId } : undefined);
  }

  getById(id: string): Observable<ProductDto> {
    return this.api.get<ProductDto>(`${this.base}/${id}`);
  }

  create(req: CreateProductRequest): Observable<ProductDto> {
    return this.api.post<ProductDto>(this.base, req);
  }

  update(id: string, req: UpdateProductRequest): Observable<ProductDto> {
    return this.api.put<ProductDto>(`${this.base}/${id}`, req);
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }

  getMovements(id: string, page: number, pageSize: number, dateFrom?: string, dateTo?: string): Observable<PagedResult<StockMovementDto>> {
    const params: Record<string, string | number> = { page, pageSize };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<PagedResult<StockMovementDto>>(`${this.base}/${id}/movements`, params);
  }

  addMovement(id: string, req: CreateStockMovementRequest): Observable<StockMovementDto> {
    return this.api.post<StockMovementDto>(`${this.base}/${id}/movements`, req);
  }
}
