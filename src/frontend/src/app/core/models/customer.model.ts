export interface CustomerDto {
    id: string;
    mainAccountCode?: string;
    code?: string;
    title: string;
    taxPayerType: number;
    cardType: number;
    taxNumber?: string;
    address?: string;
    cityId?: string | null;
    cityName?: string | null;
    districtId?: string | null;
    districtName?: string | null;
    taxOfficeId?: string | null;
    taxOfficeName?: string | null;
    postalCode?: string;
    country?: string;
    phone?: string;
    email?: string;
    website?: string;
    balance: number;
}

export interface CustomerListDto extends CustomerDto {
    createdAt: string;
    totalDebit: number;
    totalCredit: number;
}

export interface AccountTransactionDto {
    id: string;
    date: string;
    description: string;
    type: number;
    amount: number;
    currency: string;
    referenceType: string;
    referenceId?: string;
    runningBalance: number;
}
