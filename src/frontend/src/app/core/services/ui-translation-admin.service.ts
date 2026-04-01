import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface UiTranslationPairAdminDto {
  key: string;
  trId: string | null;
  valueTr: string;
  enId: string | null;
  valueEn: string;
}

export interface UiTranslationPairListResponse {
  items: UiTranslationPairAdminDto[];
  total: number;
}

@Injectable({ providedIn: 'root' })
export class UiTranslationAdminService {
  constructor(private readonly http: HttpClient) {}

  listPairs(params: { q?: string; skip?: number; take?: number }): Observable<UiTranslationPairListResponse> {
    let p = new HttpParams();
    if (params.q?.trim()) p = p.set('q', params.q.trim());
    if (params.skip != null) p = p.set('skip', String(params.skip));
    if (params.take != null) p = p.set('take', String(params.take));
    return this.http.get<UiTranslationPairListResponse>('/api/translations/admin', { params: p });
  }

  updatePair(key: string, valueTr: string, valueEn: string): Observable<void> {
    return this.http.put<void>('/api/translations/admin/pair', { key, valueTr, valueEn });
  }
}
