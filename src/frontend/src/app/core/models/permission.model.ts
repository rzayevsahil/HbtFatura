export interface PermissionDto {
    id: string;
    group: string;
    code: string;
    name: string;
}

export interface RoleDto {
    id: string;
    name: string;
    displayName?: string;
}

export interface RolePermissionDto {
    permissionCodes: string[];
}
