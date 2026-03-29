import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

export interface TaxNumberCheckResponse {
  isValidFormat: boolean;
  normalized: string | null;
  isUnique: boolean;
  message: string | null;
}

@Injectable({ providedIn: 'root' })
export class TaxNumberValidationService {
  constructor(private api: ApiService) {}

  check(params: {
    value: string;
    mode: 'customer' | 'company';
    excludeCustomerId?: string;
    firmId?: string;
  }): Observable<TaxNumberCheckResponse> {
    const q: Record<string, string | boolean | undefined> = {
      value: params.value,
      mode: params.mode
    };
    if (params.excludeCustomerId) q['excludeCustomerId'] = params.excludeCustomerId;
    if (params.firmId) q['firmId'] = params.firmId;
    return this.api.get<TaxNumberCheckResponse>('/api/tax-numbers/check', q);
  }
}
