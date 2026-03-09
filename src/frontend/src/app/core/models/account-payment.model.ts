import { AccountPaymentMethod, AccountPaymentType } from './common.model';

export interface AccountPaymentRequest {
    customerId: string;
    amount: number;
    date: string;
    paymentMethod: AccountPaymentMethod;
    cashRegisterId?: string;
    bankAccountId?: string;
    description: string;
    type: AccountPaymentType;
    invoiceId?: string | null;
}
