import { AccountPaymentMethod, AccountPaymentType } from './common.model';

export interface AccountPaymentListDto {
  id: string;
  date: string;
  type: string;
  customerId: string;
  customerTitle: string;
  amount: number;
  currency: string;
  description: string;
  createdAt: string;
}

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
