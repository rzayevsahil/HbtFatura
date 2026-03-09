export interface CashRegisterDto {
    id: string;
    firmId: string;
    name: string;
    currency: string;
    isActive: boolean;
    createdAt: string;
    balance: number;
}

export interface CashTransactionDto {
    id: string;
    date: string;
    type: number;
    amount: number;
    description: string;
    referenceType: string;
    createdAt: string;
}

export interface CreateCashRegisterRequest {
    name: string;
    currency?: string;
    firmId?: string;
}

export interface UpdateCashRegisterRequest {
    name: string;
    isActive: boolean;
}

export interface CreateCashTransactionRequest {
    date: string;
    type: number;
    amount: number;
    description: string;
}
