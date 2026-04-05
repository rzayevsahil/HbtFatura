export interface ChequeOrPromissoryDto {
    id: string;
    firmId: string;
    type: number;
    portfolioNumber: string;
    serialNumber?: string;
    bordroNumber?: string;
    bordroType?: number;
    customerId: string;
    customerTitle: string;
    amount: number;
    currency?: string;
    issueDate: string;
    dueDate: string;
    status: number;
    referenceType?: string;
    referenceId?: string;
    bankAccountId?: string;
    bankAccountName?: string;
    createdAt: string;
}

export interface CreateChequeOrPromissoryRequest {
    type: number;
    customerId: string;
    amount: number;
    issueDate: string;
    dueDate: string;
    referenceType?: string;
    referenceId?: string;
    bankAccountId?: string;
    firmId?: string;
}

export interface UpdateChequeOrPromissoryRequest {
    type: number;
    serialNumber?: string;
    customerId: string;
    amount: number;
    issueDate: string;
    dueDate: string;
    referenceType?: string;
    referenceId?: string;
    bankAccountId?: string;
}
