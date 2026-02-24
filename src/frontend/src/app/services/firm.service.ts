import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface FirmDto {
  id: string;
  name: string;
  createdAt: string;
}

export interface CreateFirmRequest {
  name: string;
  adminEmail: string;
  adminPassword: string;
  adminFullName: string;
}

export interface UpdateFirmRequest {
  name: string;
}

export interface FirmUserDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class FirmService {
  private base = '/api/firms';
  constructor(private api: ApiService) {}

  getAll(): Observable<FirmDto[]> {
    return this.api.get<FirmDto[]>(this.base);
  }

  getById(id: string): Observable<FirmDto> {
    return this.api.get<FirmDto>(`${this.base}/${id}`);
  }

  getFirmUsers(firmId: string): Observable<FirmUserDto[]> {
    return this.api.get<FirmUserDto[]>(`${this.base}/${firmId}/users`);
  }

  create(req: CreateFirmRequest): Observable<FirmDto> {
    return this.api.post<FirmDto>(this.base, req);
  }

  update(id: string, req: UpdateFirmRequest): Observable<FirmDto> {
    return this.api.put<FirmDto>(`${this.base}/${id}`, req);
  }
}
