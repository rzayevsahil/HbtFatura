export interface CompanySettingsDto {
    id: string;
    companyName: string;
    cityId?: string | null;
    cityName?: string | null;
    districtId?: string | null;
    districtName?: string | null;
    taxOfficeId?: string | null;
    taxOfficeName?: string | null;
    taxNumber?: string | null;
    address?: string | null;
    phone?: string | null;
    email?: string | null;
    website?: string | null;
    iban?: string | null;
    bankName?: string | null;
    logoUrl?: string | null;
    /** Fatura numarası seri (3 harf). Boşsa firma adının ilk 3 harfi kullanılır. */
    invoiceSerialPrefix?: string | null;
    /** İrsaliye numarası seri (3 harf). Boşsa firma adının ilk 3 harfi kullanılır. */
    deliveryNoteSerialPrefix?: string | null;
}
