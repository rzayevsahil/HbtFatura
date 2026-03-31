export interface PermissionDto {
    id: string;
    group: string;
    code: string;
    name: string;
    /** Seed / sistem yetkisi; silinemez */
    isSystem?: boolean;
}

export interface RoleDto {
    id: string;
    name: string;
    displayName?: string;
    /** Seed / sistem rolü; silinemez */
    isSystem?: boolean;
}

export interface RolePermissionDto {
    permissionCodes: string[];
}
