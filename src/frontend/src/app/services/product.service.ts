import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import { PagedResult, ProductDto, ProductListDto, CreateProductRequest, UpdateProductRequest, StockMovementDto, CreateStockMovementRequest, ProductSaleRowDto } from '../core/models';

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

  /** Aynı firmada ürün kodu kullanımda mı (form doğrulama). */
  isCodeTaken(code: string, firmId: string, excludeProductId?: string): Observable<{ taken: boolean }> {
    const params: Record<string, string> = { code, firmId };
    if (excludeProductId) params['excludeId'] = excludeProductId;
    return this.api.get<{ taken: boolean }>(`${this.base}/code-taken`, params);
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

  getSales(id: string, dateFrom?: string, dateTo?: string): Observable<ProductSaleRowDto[]> {
    const params: Record<string, string> = {};
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<ProductSaleRowDto[]>(`${this.base}/${id}/sales`, Object.keys(params).length ? params : undefined);
  }
}
