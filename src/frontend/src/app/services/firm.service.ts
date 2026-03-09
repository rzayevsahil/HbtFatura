import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

import { FirmDto, CreateFirmRequest, UpdateFirmRequest, FirmUserDto } from '../core/models';

@Injectable({ providedIn: 'root' })
export class FirmService {
  private base = '/api/firms';
  constructor(private api: ApiService) { }

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
