import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface MainAccountCodeDto {
  id: string;
  firmId?: string | null;
  code: string;
  name: string;
  sortOrder: number;
  createdAt: string;
  isSystem?: boolean;
}

export interface CreateMainAccountCodeRequest {
  code: string;
  name: string;
  sortOrder?: number;
  firmId?: string;
}

export interface UpdateMainAccountCodeRequest {
  code: string;
  name: string;
  sortOrder?: number;
}

@Injectable({ providedIn: 'root' })
export class MainAccountCodeService {
  private base = '/api/mainaccountcodes';
  constructor(private api: ApiService) {}

  getByFirm(firmId?: string): Observable<MainAccountCodeDto[]> {
    return this.api.get<MainAccountCodeDto[]>(this.base, firmId ? { firmId } : undefined);
  }

  getById(id: string): Observable<MainAccountCodeDto> {
    return this.api.get<MainAccountCodeDto>(`${this.base}/${id}`);
  }

  create(req: CreateMainAccountCodeRequest): Observable<MainAccountCodeDto> {
    return this.api.post<MainAccountCodeDto>(this.base, req);
  }

  update(id: string, req: UpdateMainAccountCodeRequest): Observable<MainAccountCodeDto> {
    return this.api.put<MainAccountCodeDto>(`${this.base}/${id}`, req);
  }

  delete(id: string): Observable<void> {
    return this.api.delete(`${this.base}/${id}`);
  }
}
