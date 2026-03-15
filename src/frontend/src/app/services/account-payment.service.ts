import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import { PagedResult } from '../core/models/pagination.model';
import { AccountPaymentRequest, AccountPaymentListDto } from '../core/models';

@Injectable({ providedIn: 'root' })
export class AccountPaymentService {
  private base = '/api/account-payments';
  constructor(private api: ApiService) { }

  getPaged(params: {
    page?: number;
    pageSize?: number;
    dateFrom?: string;
    dateTo?: string;
    customerId?: string;
    type?: string;
    firmId?: string;
  }): Observable<PagedResult<AccountPaymentListDto>> {
    const query: Record<string, string> = {};
    if (params.page != null) query['page'] = String(params.page);
    if (params.pageSize != null) query['pageSize'] = String(params.pageSize);
    if (params.dateFrom) query['dateFrom'] = params.dateFrom;
    if (params.dateTo) query['dateTo'] = params.dateTo;
    if (params.customerId) query['customerId'] = params.customerId;
    if (params.type) query['type'] = params.type;
    if (params.firmId) query['firmId'] = params.firmId;
    return this.api.get<PagedResult<AccountPaymentListDto>>(this.base, query);
  }

  create(req: AccountPaymentRequest): Observable<void> {
    return this.api.post<void>(this.base, req);
  }
}
