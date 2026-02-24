import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface EmployeeListDto {
  id: string;
  email: string;
  fullName: string;
  createdAt: string;
}

export interface CreateEmployeeRequest {
  email: string;
  password: string;
  fullName: string;
}

@Injectable({ providedIn: 'root' })
export class EmployeeService {
  private base = '/api/employees';
  constructor(private api: ApiService) {}

  getAll(): Observable<EmployeeListDto[]> {
    return this.api.get<EmployeeListDto[]>(this.base);
  }

  create(req: CreateEmployeeRequest): Observable<EmployeeListDto> {
    return this.api.post<EmployeeListDto>(this.base, req);
  }
}
