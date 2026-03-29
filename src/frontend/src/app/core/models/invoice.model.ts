import { InvoiceType } from './common.model';

export type InvoiceStatus = 0 | 1 | 2 | 3 | 4; // Draft, Issued, Paid, Cancelled, PendingGibAcceptance
export type InvoiceScenario = 0 | 1; // TemelFatura, TicariFatura

export interface InvoiceItemDto {
    id?: string;
    productId?: string;
    productCode?: string;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    discountPercent: number;
    lineTotalExclVat: number;
    lineVatAmount: number;
    lineTotalInclVat: number;
    sortOrder: number;
}

export interface InvoiceItemInputDto {
    productId?: string;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    discountPercent: number;
    sortOrder: number;
}

export interface InvoiceDto {
    id: string;
    invoiceNumber: string;
    invoiceDate: string;
    status: InvoiceStatus;
    invoiceType: InvoiceType;
    scenario: InvoiceScenario;
    customerId?: string;
    customerTitle: string;
    customerTaxNumber?: string;
    customerAddress?: string;
    customerPhone?: string;
    customerEmail?: string;
    customerWebsite?: string;
    customerTaxOffice?: string;
    subTotal: number;
    totalVat: number;
    grandTotal: number;
    currency: string;
    exchangeRate: number;
    items: InvoiceItemDto[];
    sourceType?: string | null;
    sourceId?: string | null;
    sourceNumber?: string | null;
    isGibSent?: boolean;
    createdByUserId?: string;
    createdByUserName?: string | null;
}

export interface InvoiceListDto {
    id: string;
    invoiceNumber: string;
    invoiceDate: string;
    status: InvoiceStatus;
    invoiceType: InvoiceType;
    customerId?: string;
    customerTitle: string;
    grandTotal: number;
    currency: string;
    isGibSent: boolean;
    sourceType?: string | null;
    createdByUserId: string;
    createdByUserName?: string | null;
}

export interface CreateInvoiceRequest {
    invoiceDate: string;
    invoiceType?: InvoiceType;
    customerId?: string;
    customerTitle: string;
    customerTaxNumber?: string;
    customerAddress?: string;
    customerPhone?: string;
    customerEmail?: string;
    customerWebsite?: string;
    customerTaxOffice?: string;
    currency: string;
    exchangeRate: number;
    deliveryNoteId?: string;
    items: InvoiceItemInputDto[];
}
