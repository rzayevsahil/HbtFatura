export interface MenuItem {
    id: string;
    parentId?: string;
    label: string;
    icon?: string;
    routerLink?: string;
    sortOrder: number;
    requiredPermissionCode?: string;
    isActive: boolean;
    isSystemMenu: boolean;
    children: MenuItem[];
}
