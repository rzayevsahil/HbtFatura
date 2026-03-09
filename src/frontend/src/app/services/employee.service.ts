import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

import { EmployeeListDto, CreateEmployeeRequest } from '../core/models';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private base = '/api/employees';
  constructor(private api: ApiService) { }

  getAll(): Observable<EmployeeListDto[]> {
    return this.api.get<EmployeeListDto[]>(this.base);
  }

  create(req: CreateEmployeeRequest): Observable<EmployeeListDto> {
    return this.api.post<EmployeeListDto>(this.base, req);
  }

  getById(id: string): Observable<EmployeeListDto> {
    return this.api.get<EmployeeListDto>(`${this.base}/${id}`);
  }

  update(id: string, req: Partial<CreateEmployeeRequest>): Observable<EmployeeListDto> {
    return this.api.put<EmployeeListDto>(`${this.base}/${id}`, req);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.base}/${id}`);
  }
}
