/** API enum: 0=Satis, 1=Alis. Siparişte 0=Alınan, 1=Verilen; irsaliye/faturada 0=Satış, 1=Alış (aynı sayısal değerler). */
export type InvoiceType = 0 | 1;
export type VatRate = 0 | 1 | 8 | 10 | 18 | 20;
export type LogLevel = 'Info' | 'Warning' | 'Error';
export type CurrencyCode = 'TRY' | 'USD' | 'EUR';
export type SortOrder = 'asc' | 'desc';
export type StockMovementType = 1 | 2; // 1: Giriş (Entry), 2: Çıkış (Exit)
export type AccountPaymentType = 'Tahsilat' | 'Ödeme';
export type AccountPaymentMethod = 'Kasa' | 'Banka';
