import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface AccountPaymentRequest {
  customerId: string;
  amount: number;
  date: string;
  paymentMethod: string;
  cashRegisterId?: string;
  bankAccountId?: string;
  description: string;
  type: string;
  invoiceId?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AccountPaymentService {
  private base = '/api/account-payments';
  constructor(private api: ApiService) {}

  create(req: AccountPaymentRequest): Observable<void> {
    return this.api.post<void>(this.base, req);
  }
}
