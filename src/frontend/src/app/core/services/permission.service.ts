import { Injectable, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface PermissionDto {
    id: string;
    group: string;
    code: string;
    name: string;
}

export interface RoleDto {
    id: string;
    name: string;
}

@Injectable({ providedIn: 'root' })
export class PermissionService {
    private userPermissions = computed(() => this.auth.user()?.permissions || []);

    constructor(private auth: AuthService, private http: HttpClient) { }

    hasPermission(permission: string): boolean {
        const perms = this.userPermissions();
        if (!perms || perms.length === 0) return false;


        return perms.includes(permission);
    }

    hasAnyPermission(permissions: string[]): boolean {
        return permissions.some(p => this.hasPermission(p));
    }

    // Permission Management
    getPermissions(): Observable<PermissionDto[]> {
        return this.http.get<PermissionDto[]>('/api/permissions');
    }

    createPermission(permission: Partial<PermissionDto>): Observable<PermissionDto> {
        return this.http.post<PermissionDto>('/api/permissions', permission);
    }

    updatePermission(id: string, permission: Partial<PermissionDto>): Observable<void> {
        return this.http.put<void>(`/api/permissions/${id}`, permission);
    }

    deletePermission(id: string): Observable<void> {
        return this.http.delete<void>(`/api/permissions/${id}`);
    }

    // Role Management
    getRoles(): Observable<RoleDto[]> {
        return this.http.get<RoleDto[]>('/api/roles');
    }

    createRole(name: string): Observable<RoleDto> {
        return this.http.post<RoleDto>('/api/roles', JSON.stringify(name), {
            headers: { 'Content-Type': 'application/json' }
        });
    }

    updateRole(id: string, name: string): Observable<void> {
        return this.http.put<void>(`/api/roles/${id}`, JSON.stringify(name), {
            headers: { 'Content-Type': 'application/json' }
        });
    }

    deleteRole(id: string): Observable<void> {
        return this.http.delete<void>(`/api/roles/${id}`);
    }

    // Role-Permission mapping
    getRolePermissions(roleId: string): Observable<string[]> {
        return this.http.get<string[]>(`/api/permissions/role/${roleId}`);
    }

    updateRolePermissions(roleId: string, permissionCodes: string[]): Observable<void> {
        return this.http.post<void>(`/api/permissions/role/${roleId}`, { permissionCodes });
    }
}
