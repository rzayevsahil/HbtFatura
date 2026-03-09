export interface LookupGroupDto {
    id: string;
    name: string;        // e.g. "OrderType"
    displayName: string; // e.g. "Sipariş Tipi"
    description?: string;
    isSystemGroup: boolean;
}

export interface LookupDto {
    id: string;
    lookupGroupId: string;
    group?: LookupGroupDto;
    code: string;
    name: string;
    color?: string;
    sortOrder: number;
    isActive: boolean;
}
