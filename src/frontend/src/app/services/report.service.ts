import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

function buildCariExtractQuery(customerId: string, format: string, dateFrom?: string, dateTo?: string): string {
  const p = new URLSearchParams({ customerId, format });
  if (dateFrom) p.set('dateFrom', dateFrom);
  if (dateTo) p.set('dateTo', dateTo);
  return p.toString();
}

export interface CariExtractRowDto {
  date: string;
  description: string;
  borc: number;
  alacak: number;
  bakiye: number;
}

export interface CariExtractReportDto {
  customerId: string;
  customerTitle: string;
  dateFrom?: string;
  dateTo?: string;
  openingBalance: number;
  closingBalance: number;
  rows: CariExtractRowDto[];
}

export interface CashSummaryRowDto {
  date: string;
  description: string;
  type: number;
  amount: number;
}

export interface CashSummaryReportDto {
  cashRegisterId?: string;
  cashRegisterName?: string;
  dateFrom?: string;
  dateTo?: string;
  openingBalance: number;
  totalGiris: number;
  totalCikis: number;
  closingBalance: number;
  rows: CashSummaryRowDto[];
}

export interface BankSummaryRowDto {
  date: string;
  description: string;
  type: number;
  amount: number;
}

export interface BankSummaryReportDto {
  bankAccountId?: string;
  bankAccountName?: string;
  dateFrom?: string;
  dateTo?: string;
  openingBalance: number;
  totalGiris: number;
  totalCikis: number;
  closingBalance: number;
  rows: BankSummaryRowDto[];
}

export interface StockLevelRowDto {
  code: string;
  name: string;
  unit: string;
  quantity: number;
  minStock: number;
  maxStock: number;
  lowStock: boolean;
}

export interface StockLevelsReportDto {
  items: StockLevelRowDto[];
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private base = '/api/reports';
  constructor(private api: ApiService) {}

  getCariExtract(customerId: string, dateFrom?: string, dateTo?: string): Observable<CariExtractReportDto> {
    const params: Record<string, string> = { customerId };
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<CariExtractReportDto>(`${this.base}/cari-extract`, params);
  }

  downloadCariExtractPdf(customerId: string, dateFrom?: string, dateTo?: string): Observable<Blob> {
    const q = buildCariExtractQuery(customerId, 'pdf', dateFrom, dateTo);
    return this.api.getBlob(`${this.base}/cari-extract?${q}`);
  }

  downloadCariExtractExcel(customerId: string, dateFrom?: string, dateTo?: string): Observable<Blob> {
    const q = buildCariExtractQuery(customerId, 'xlsx', dateFrom, dateTo);
    return this.api.getBlob(`${this.base}/cari-extract?${q}`);
  }

  getCashSummary(cashRegisterId?: string, dateFrom?: string, dateTo?: string): Observable<CashSummaryReportDto> {
    const params: Record<string, string> = {};
    if (cashRegisterId) params['cashRegisterId'] = cashRegisterId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<CashSummaryReportDto>(`${this.base}/cash-summary`, params);
  }

  getBankSummary(bankAccountId?: string, dateFrom?: string, dateTo?: string): Observable<BankSummaryReportDto> {
    const params: Record<string, string> = {};
    if (bankAccountId) params['bankAccountId'] = bankAccountId;
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    return this.api.get<BankSummaryReportDto>(`${this.base}/bank-summary`, params);
  }

  getStockLevels(firmId?: string): Observable<StockLevelsReportDto> {
    const params: Record<string, string> = {};
    if (firmId) params['firmId'] = firmId;
    return this.api.get<StockLevelsReportDto>(`${this.base}/stock-levels`, params);
  }
}
