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
}
