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
  productId: string;
  code: string;
  name: string;
  unit: string;
  quantity: number;
}

export interface StockLevelsReportDto {
  items: StockLevelRowDto[];
}

export interface InvoiceReportRowDto {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  status: number;
  customerTitle: string;
  grandTotal: number;
  currency: string;
}

export interface InvoiceReportDto {
  dateFrom?: string;
  dateTo?: string;
  customerId?: string;
  customerTitle?: string;
  items: InvoiceReportRowDto[];
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private base = '/api/reports';
  constructor(private api: ApiService) { }

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

  getInvoiceReport(dateFrom?: string, dateTo?: string, customerId?: string): Observable<InvoiceReportDto> {
    const params: Record<string, string> = {};
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    if (customerId) params['customerId'] = customerId;
    return this.api.get<InvoiceReportDto>(`${this.base}/invoice-report`, params);
  }

  downloadInvoiceReportPdf(dateFrom?: string, dateTo?: string, customerId?: string): Observable<Blob> {
    const p = new URLSearchParams({ format: 'pdf' });
    if (dateFrom) p.set('dateFrom', dateFrom);
    if (dateTo) p.set('dateTo', dateTo);
    if (customerId) p.set('customerId', customerId);
    return this.api.getBlob(`${this.base}/invoice-report?${p.toString()}`);
  }

  downloadInvoiceReportExcel(dateFrom?: string, dateTo?: string, customerId?: string): Observable<Blob> {
    const p = new URLSearchParams({ format: 'xlsx' });
    if (dateFrom) p.set('dateFrom', dateFrom);
    if (dateTo) p.set('dateTo', dateTo);
    if (customerId) p.set('customerId', customerId);
    return this.api.getBlob(`${this.base}/invoice-report?${p.toString()}`);
  }
}
