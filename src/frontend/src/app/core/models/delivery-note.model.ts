import { InvoiceType } from './common.model';
export type DeliveryNoteStatus = 0 | 1 | 2 | 3; // Taslak, Onaylandi, Iptal, Faturalandi

export interface DeliveryNoteItemDto {
    id: string;
    productId?: string;
    productCode?: string;
    orderItemId?: string;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    sortOrder: number;
}

export interface DeliveryNoteDto {
    id: string;
    deliveryNumber: string;
    customerId?: string;
    customerTitle?: string;
    orderId?: string;
    orderNumber?: string;
    invoiceId?: string | null;
    deliveryDate: string;
    status: DeliveryNoteStatus | string;
    deliveryType: InvoiceType;
    createdAt: string;
    items: DeliveryNoteItemDto[];
}

export interface DeliveryNoteListDto {
    id: string;
    deliveryNumber: string;
    deliveryDate: string;
    status: DeliveryNoteStatus | string;
    deliveryType: InvoiceType;
    customerId?: string;
    customerTitle?: string;
    orderNumber?: string;
    invoiceId?: string | null;
    createdByUserId: string;
    createdByUserName?: string | null;
}

export interface CreateDeliveryNoteFromOrderRequest {
    orderId: string;
    deliveryDate: string;
}

export interface DeliveryNoteItemInputDto {
    productId?: string | null;
    orderItemId?: string | null;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    sortOrder: number;
}

export interface CreateDeliveryNoteRequest {
    customerId?: string | null;
    orderId?: string | null;
    deliveryDate: string;
    deliveryType: InvoiceType;
    items: DeliveryNoteItemInputDto[];
}

export interface UpdateDeliveryNoteRequest {
    customerId?: string | null;
    deliveryDate: string;
    items: DeliveryNoteItemInputDto[];
}
