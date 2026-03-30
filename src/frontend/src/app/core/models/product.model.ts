import { StockMovementType, CurrencyCode } from './common.model';

export interface ProductDto {
    id: string;
    firmId: string;
    code: string;
    name: string;
    barcode?: string;
    stockType: string;
    unit: string;
    stockQuantity: number;
    unitPrice: number;
    currency: CurrencyCode;
    createdAt: string;
}

export interface ProductListDto extends ProductDto { }

export interface CreateProductRequest {
    code: string;
    name: string;
    barcode?: string;
    stockType?: string;
    unit?: string;
    stockQuantity?: number;
    unitPrice?: number;
    currency?: CurrencyCode;
    firmId?: string;
}

export interface UpdateProductRequest {
    code: string;
    name: string;
    barcode?: string;
    stockType?: string;
    unit?: string;
    stockQuantity?: number;
    unitPrice?: number;
    currency?: CurrencyCode;
}

export interface StockMovementDto {
    id: string;
    date: string;
    type: StockMovementType;
    quantity: number;
    referenceType: string;
    referenceId?: string;
    invoiceId?: string;
    invoiceNumber?: string;
    deliveryNoteId?: string;
    deliveryNumber?: string;
    orderId?: string;
    orderNumber?: string;
    createdAt: string;
}

export interface CreateStockMovementRequest {
    date: string;
    type: StockMovementType;
    quantity: number;
    description: string;
}

export interface ProductSaleRowDto {
    date: string;
    quantity: number;
    invoiceNumber: string;
    invoiceId?: string;
    orderNumber?: string;
    orderId?: string;
    deliveryNumber?: string;
    deliveryNoteId?: string;
}
