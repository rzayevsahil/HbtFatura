import { ChangeDetectorRef, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { PermissionService } from '../../core/services/permission.service';
import { PermissionDto, RoleDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-permission-management',
    standalone: true,
    imports: [CommonModule, FormsModule, TranslateModule],
    templateUrl: './permission-management.component.html',
    styleUrls: ['./permission-management.component.scss']
})
export class PermissionManagementComponent implements OnInit {
    private readonly destroyRef = inject(DestroyRef);

    roles = signal<RoleDto[]>([]);
    permissions = signal<PermissionDto[]>([]);
    groupedPermissions = signal<{ group: string, perms: PermissionDto[] }[]>([]);

    selectedRoleId = signal<string | null>(null);
    rolePermissions = signal<string[]>([]);
    loading = signal(false);
    saving = signal(false);

    // CRUD Modals
    showRoleModal = signal(false);
    editingRole = signal<{ id?: string, name: string, displayName?: string } | null>(null);

    showPermModal = signal(false);
    editingPerm = signal<Partial<PermissionDto> | null>(null);

    constructor(
        private permService: PermissionService,
        private toastr: ToastrService,
        private translate: TranslateService,
        private cdr: ChangeDetectorRef
    ) {
        this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
    }

    ngOnInit() {
        this.loadInitialData();
    }

    /** API’de saklanan Türkçe grup adından yerelleştirilmiş grup başlığı. */
    displayPermissionGroup(group: string): string {
        const key = `permissionManagement.groups.${group}`;
        const t = this.translate.instant(key);
        return t === key ? group : t;
    }

    /** Yetki koduna (Module.Action) göre yerelleştirilmiş kısa ad; yoksa API `name`. */
    displayPermissionLabel(perm: PermissionDto): string {
        const dot = perm.code.indexOf('.');
        if (dot <= 0) return perm.name;
        const mod = perm.code.slice(0, dot);
        const action = perm.code.slice(dot + 1);
        const key = `permissionManagement.perms.${mod}.${action}`;
        const t = this.translate.instant(key);
        return t === key ? perm.name : t;
    }

    /** Yerleşik roller için i18n; özel rollerde `displayName` / `name`. */
    displayRoleLabel(role: RoleDto): string {
        const key = `permissionManagement.roleLabels.${role.name}`;
        const t = this.translate.instant(key);
        return t === key ? (role.displayName || role.name) : t;
    }

    getSelectedRoleDisplayLabel(): string {
        const role = this.roles().find(r => r.id === this.selectedRoleId());
        return role ? this.displayRoleLabel(role) : '';
    }

    loadInitialData() {
        this.loading.set(true);
        this.permService.getRoles().subscribe({
            next: (roles) => {
                this.roles.set(roles);
                if (roles.length > 0 && !this.selectedRoleId()) {
                    this.selectRole(roles[0].id);
                }
            },
            error: () => this.toastr.error(this.translate.instant('permissionManagement.toasts.rolesLoadError'))
        });

        this.permService.getPermissions().subscribe({
            next: (perms) => {
                this.permissions.set(perms);
                this.updateGroupedPermissions(perms);
                this.loading.set(false);
            },
            error: () => {
                this.toastr.error(this.translate.instant('permissionManagement.toasts.permissionsLoadError'));
                this.loading.set(false);
            }
        });
    }

    updateGroupedPermissions(perms: PermissionDto[]) {
        const groups = Array.from(new Set(perms.map(p => p.group)));
        this.groupedPermissions.set(groups.map(g => ({
            group: g,
            perms: perms.filter(p => p.group === g)
        })));
    }

    selectRole(roleId: string) {
        this.selectedRoleId.set(roleId);
        this.permService.getRolePermissions(roleId).subscribe({
            next: (perms) => this.rolePermissions.set(perms),
            error: () => this.toastr.error(this.translate.instant('permissionManagement.toasts.rolePermissionsLoadError'))
        });
    }

    togglePermission(code: string) {
        const current = this.rolePermissions();
        if (current.includes(code)) {
            this.rolePermissions.set(current.filter(c => c !== code));
        } else {
            this.rolePermissions.set([...current, code]);
        }
    }

    isPermissionSelected(code: string): boolean {
        return this.rolePermissions().includes(code);
    }

    saveRolePermissions() {
        const roleId = this.selectedRoleId();
        if (!roleId) return;

        this.saving.set(true);
        this.permService.updateRolePermissions(roleId, this.rolePermissions()).subscribe({
            next: () => {
                this.toastr.success(this.translate.instant('permissionManagement.toasts.permissionsUpdated'));
                this.saving.set(false);
            },
            error: (err) => {
                this.toastr.error(err.error?.message || this.translate.instant('permissionManagement.toasts.permissionsUpdateError'));
                this.saving.set(false);
            }
        });
    }

    // Role CRUD
    openRoleModal(role?: RoleDto) {
        this.editingRole.set(role ? { ...role } : { name: '', displayName: '' });
        this.showRoleModal.set(true);
    }

    saveRole() {
        const role = this.editingRole();
        if (!role || !role.name) return;

        if (role.id) {
            this.permService.updateRole(role.id, role).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('permissionManagement.toasts.roleUpdated'));
                    this.loadInitialData();
                    this.showRoleModal.set(false);
                },
                error: (err) => this.toastr.error(err.error?.message || this.translate.instant('permissionManagement.toasts.roleUpdateError'))
            });
        } else {
            this.permService.createRole(role).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('permissionManagement.toasts.roleCreated'));
                    this.loadInitialData();
                    this.showRoleModal.set(false);
                },
                error: (err) => this.toastr.error(err.error?.message || this.translate.instant('permissionManagement.toasts.roleCreateError'))
            });
        }
    }

    deleteRole(id: string) {
        if (confirm(this.translate.instant('permissionManagement.confirmDeleteRole'))) {
            this.permService.deleteRole(id).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('permissionManagement.toasts.roleDeleted'));
                    if (this.selectedRoleId() === id) {
                        this.selectedRoleId.set(null);
                        this.rolePermissions.set([]);
                    }
                    this.loadInitialData();
                },
                error: (err) => this.toastr.error(err.error?.message || this.translate.instant('permissionManagement.toasts.roleDeleteError'))
            });
        }
    }

    // Permission CRUD
    openPermModal() {
        this.editingPerm.set({ group: '', code: '', name: '' });
        this.showPermModal.set(true);
    }

    savePermission() {
        const perm = this.editingPerm();
        if (!perm || !perm.code || !perm.name) return;

        this.permService.createPermission(perm).subscribe({
            next: () => {
                this.toastr.success(this.translate.instant('permissionManagement.toasts.permissionCreated'));
                this.loadInitialData();
                this.showPermModal.set(false);
            },
            error: (err) => this.toastr.error(err.error || this.translate.instant('permissionManagement.toasts.permissionCreateError'))
        });
    }

    deletePermission(id: string) {
        if (confirm(this.translate.instant('permissionManagement.confirmDeletePermission'))) {
            this.permService.deletePermission(id).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('permissionManagement.toasts.permissionDeleted'));
                    this.loadInitialData();
                },
                error: (err) => this.toastr.error(err.error || this.translate.instant('permissionManagement.toasts.permissionDeleteError'))
            });
        }
    }

    getSelectedRoleName(): string {
        const role = this.roles().find(r => r.id === this.selectedRoleId());
        return role?.displayName || role?.name || '';
    }
}
