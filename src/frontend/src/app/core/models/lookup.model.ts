export interface LookupGroupDto {
    id: string;
    name: string;        // e.g. "OrderType"
    displayName: string; // e.g. "Sipariş Tipi" (TR)
    displayNameEn?: string | null;
    description?: string;
    isSystemGroup: boolean;
}

export interface LookupDto {
    id: string;
    lookupGroupId: string;
    group?: LookupGroupDto;
    code: string;
    name: string;
    /** İngilizce görünen ad (sistem tanımları / EN dil) */
    nameEn?: string | null;
    color?: string;
    sortOrder: number;
    isActive: boolean;
}
