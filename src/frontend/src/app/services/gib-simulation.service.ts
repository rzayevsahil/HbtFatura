import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

export interface GibInboxItemDto {
  submissionId: string;
  invoiceId: string;
  invoiceNumber: string;
  invoiceDate: string;
  senderFirmName: string;
  customerTitle: string;
  recipientTaxNumber: string;
  grandTotal: number;
  currency: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class GibSimulationService {
  private base = '/api/GibSimulation';
  constructor(private api: ApiService) { }

  inbox(): Observable<GibInboxItemDto[]> {
    return this.api.get<GibInboxItemDto[]>(`${this.base}/inbox`);
  }

  accept(submissionId: string): Observable<void> {
    return this.api.post<void>(`${this.base}/${submissionId}/accept`, {});
  }

  reject(submissionId: string): Observable<void> {
    return this.api.post<void>(`${this.base}/${submissionId}/reject`, {});
  }
}
