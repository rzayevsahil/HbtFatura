import { StockMovementType } from './common.model';

export interface ProductDto {
    id: string;
    firmId: string;
    code: string;
    name: string;
    barcode?: string;
    unit: string;
    stockQuantity: number;
    unitPrice: number;
    createdAt: string;
}

export interface ProductListDto extends ProductDto { }

export interface CreateProductRequest {
    code: string;
    name: string;
    barcode?: string;
    unit?: string;
    stockQuantity?: number;
    unitPrice?: number;
    firmId?: string;
}

export interface UpdateProductRequest {
    code: string;
    name: string;
    barcode?: string;
    unit?: string;
    stockQuantity?: number;
    unitPrice?: number;
}

export interface StockMovementDto {
    id: string;
    date: string;
    type: StockMovementType;
    quantity: number;
    referenceType: string;
    referenceId?: string;
    createdAt: string;
}

export interface CreateStockMovementRequest {
    date: string;
    type: StockMovementType;
    quantity: number;
    description: string;
}
