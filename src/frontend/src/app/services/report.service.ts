import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';

function buildCariExtractQuery(customerId: string, format: string, dateFrom?: string, dateTo?: string): string {
  const p = new URLSearchParams({ customerId, format });
  if (dateFrom) p.set('dateFrom', dateFrom);
  if (dateTo) p.set('dateTo', dateTo);
  return p.toString();
}

import {
  CariExtractReportDto, CashSummaryReportDto, BankSummaryReportDto,
  StockLevelsReportDto, InvoiceReportDto, MonthlyProductSalesReportDto
} from '../core/models';

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

  downloadStockLevelsPdf(firmId?: string): Observable<Blob> {
    const p = new URLSearchParams({ format: 'pdf' });
    if (firmId) p.set('firmId', firmId);
    return this.api.getBlob(`${this.base}/stock-levels?${p.toString()}`);
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

  getMonthlyProductSales(dateFrom?: string, dateTo?: string, productId?: string): Observable<MonthlyProductSalesReportDto> {
    const params: Record<string, string> = {};
    if (dateFrom) params['dateFrom'] = dateFrom;
    if (dateTo) params['dateTo'] = dateTo;
    if (productId) params['productId'] = productId;
    return this.api.get<MonthlyProductSalesReportDto>(`${this.base}/monthly-product-sales`, Object.keys(params).length ? params : undefined);
  }
}
