import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

import { AccountPaymentRequest } from '../core/models';

@Injectable({ providedIn: 'root' })
export class AccountPaymentService {
  private base = '/api/account-payments';
  constructor(private api: ApiService) { }

  create(req: AccountPaymentRequest): Observable<void> {
    return this.api.post<void>(this.base, req);
  }
}
