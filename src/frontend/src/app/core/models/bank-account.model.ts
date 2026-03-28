export interface BankAccountDto {
    id: string;
    firmId: string;
    name: string;
    iban?: string;
    bankName?: string;
    currency: string;
    isActive: boolean;
    createdAt: string;
    balance: number;
}

export interface BankTransactionDto {
    id: string;
    date: string;
    type: number;
    amount: number;
    description: string;
    referenceType: string;
    createdAt: string;
}

export interface CreateBankAccountRequest {
    name: string;
    iban?: string;
    bankName?: string;
    currency?: string;
    firmId?: string;
}

export interface UpdateBankAccountRequest {
    name: string;
    iban?: string;
    bankName?: string;
    currency: string;
    isActive: boolean;
}

export interface CreateBankTransactionRequest {
    date: string;
    type: number;
    amount: number;
    description: string;
}
