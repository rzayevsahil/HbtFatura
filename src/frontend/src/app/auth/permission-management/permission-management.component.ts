import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PermissionService } from '../../core/services/permission.service';
import { PermissionDto, RoleDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-permission-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './permission-management.component.html',
    styleUrls: ['./permission-management.component.scss']
})
export class PermissionManagementComponent implements OnInit {
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
        private toastr: ToastrService
    ) { }

    ngOnInit() {
        this.loadInitialData();
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
            error: () => this.toastr.error('Roller yüklenemedi.')
        });

        this.permService.getPermissions().subscribe({
            next: (perms) => {
                this.permissions.set(perms);
                this.updateGroupedPermissions(perms);
                this.loading.set(false);
            },
            error: () => {
                this.toastr.error('Yetkiler yüklenemedi.');
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
            error: () => this.toastr.error('Rol yetkileri yüklenemedi.')
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
                this.toastr.success('Yetkiler başarıyla güncellendi.');
                this.saving.set(false);
            },
            error: (err) => {
                this.toastr.error(err.error?.message || 'Yetkiler güncellenirken hata oluştu.');
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
                    this.toastr.success('Rol güncellendi.');
                    this.loadInitialData();
                    this.showRoleModal.set(false);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Rol güncellenemedi.')
            });
        } else {
            this.permService.createRole(role).subscribe({
                next: () => {
                    this.toastr.success('Rol oluşturuldu.');
                    this.loadInitialData();
                    this.showRoleModal.set(false);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Rol oluşturulamadı.')
            });
        }
    }

    deleteRole(id: string) {
        if (confirm('Bu rolü silmek istediğinize emin misiniz?')) {
            this.permService.deleteRole(id).subscribe({
                next: () => {
                    this.toastr.success('Rol silindi.');
                    if (this.selectedRoleId() === id) {
                        this.selectedRoleId.set(null);
                        this.rolePermissions.set([]);
                    }
                    this.loadInitialData();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Rol silinemedi.')
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
                this.toastr.success('Yetki oluşturuldu.');
                this.loadInitialData();
                this.showPermModal.set(false);
            },
            error: (err) => this.toastr.error(err.error || 'Yetki oluşturulamadı.')
        });
    }

    deletePermission(id: string) {
        if (confirm('Bu yetkiyi silmek istediğinize emin misiniz?')) {
            this.permService.deletePermission(id).subscribe({
                next: () => {
                    this.toastr.success('Yetki silindi.');
                    this.loadInitialData();
                },
                error: (err) => this.toastr.error(err.error || 'Yetki silinemedi.')
            });
        }
    }

    getSelectedRoleName(): string {
        const role = this.roles().find(r => r.id === this.selectedRoleId());
        return role?.displayName || role?.name || '';
    }
}
