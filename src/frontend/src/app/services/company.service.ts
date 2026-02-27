import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface CompanySettingsDto {
  id: string;
  companyName: string;
  taxOffice?: string;
  taxNumber?: string;
  address?: string;
  phone?: string;
  email?: string;
  iban?: string;
  bankName?: string;
  logoUrl?: string;
}

@Injectable({ providedIn: 'root' })
export class CompanyService {
  private base = '/api/companysettings';
  constructor(private api: ApiService) { }

  get(firmId?: string): Observable<CompanySettingsDto> {
    const params = firmId ? { firmId } : undefined;
    return this.api.get<CompanySettingsDto>(this.base, params);
  }

  save(dto: Partial<CompanySettingsDto>, firmId?: string): Observable<CompanySettingsDto> {
    const params = firmId ? { firmId } : undefined;
    const path = params ? `${this.base}?firmId=${encodeURIComponent(firmId!)}` : this.base;
    return this.api.put<CompanySettingsDto>(path, dto);
  }
}
