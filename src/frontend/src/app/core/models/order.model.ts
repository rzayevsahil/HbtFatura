import { InvoiceType } from './common.model';
export type OrderStatus = 0 | 1 | 2 | 3 | 4; // Bekliyor, TamamiTeslim, Iptal, Onaylandi, KismiTeslim

export interface OrderItemDto {
    id: string;
    productId?: string;
    productCode?: string;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    sortOrder: number;
}

export interface OrderDto {
    id: string;
    orderNumber: string;
    customerId?: string;
    customerTitle?: string;
    orderDate: string;
    status: OrderStatus | string;
    orderType: InvoiceType;
    createdAt: string;
    createdByUserId?: string;
    createdByUserName?: string | null;
    deliveryNoteId?: string;
    deliveryNoteNumber?: string;
    items: OrderItemDto[];
}

export interface OrderListDto {
    id: string;
    orderNumber: string;
    orderDate: string;
    status: OrderStatus | string;
    orderType: InvoiceType;
    customerId?: string;
    customerTitle?: string;
    totalAmount?: number;
    createdByUserId: string;
    createdByUserName?: string | null;
}

export interface OrderItemInputDto {
    productId?: string;
    description: string;
    unit: string;
    quantity: number;
    unitPrice: number;
    vatRate: number;
    sortOrder: number;
}

export interface CreateOrderRequest {
    customerId?: string;
    orderDate: string;
    orderType?: InvoiceType;
    items: OrderItemInputDto[];
}

export interface UpdateOrderRequest {
    customerId?: string;
    orderDate: string;
    status?: OrderStatus;
    items: OrderItemInputDto[];
}
