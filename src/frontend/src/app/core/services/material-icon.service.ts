import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MaterialIconDto, CreateMaterialIconRequest } from '../models/material-icon.model';

@Injectable({ providedIn: 'root' })
export class MaterialIconService {
  constructor(private http: HttpClient) {}

  /** İkon seçici (aktif ligature adları). */
  getForPicker(): Observable<string[]> {
    return this.http.get<string[]>('/api/material-icons/for-picker');
  }

  getAll(): Observable<MaterialIconDto[]> {
    return this.http.get<MaterialIconDto[]>('/api/material-icons');
  }

  create(body: CreateMaterialIconRequest): Observable<MaterialIconDto> {
    return this.http.post<MaterialIconDto>('/api/material-icons', body);
  }

  update(id: string, body: CreateMaterialIconRequest): Observable<MaterialIconDto> {
    return this.http.put<MaterialIconDto>(`/api/material-icons/${id}`, body);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`/api/material-icons/${id}`);
  }
}
