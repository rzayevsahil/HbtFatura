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
