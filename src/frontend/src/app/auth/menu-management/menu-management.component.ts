import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MenuService } from '../../core/services/menu.service';
import { PermissionService } from '../../core/services/permission.service';
import { MenuItem, PermissionDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app-menu-management',
    standalone: true,
    imports: [CommonModule, FormsModule, TranslateModule],
    templateUrl: './menu-management.component.html',
    styleUrls: ['./menu-management.component.scss']
})
export class MenuManagementComponent implements OnInit {
    allMenus = signal<MenuItem[]>([]);
    flatMenus = signal<MenuItem[]>([]);
    permissions = signal<PermissionDto[]>([]);
    loading = signal(false);

    showModal = signal(false);
    editingMenu = signal<Partial<MenuItem> | null>(null);

    constructor(
        private menuService: MenuService,
        private permService: PermissionService,
        private toastr: ToastrService,
        private translate: TranslateService
    ) { }

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.loading.set(true);
        this.menuService.getAllMenus().subscribe({
            next: (menus) => {
                this.allMenus.set(menus);
                this.flatMenus.set(this.flattenMenus(menus));
                this.loading.set(false);
            },
            error: () => {
                this.toastr.error(this.translate.instant('menuManagement.toasts.loadError'));
                this.loading.set(false);
            }
        });

        this.permService.getPermissions().subscribe({
            next: (perms) => this.permissions.set(perms)
        });
    }

    flattenMenus(menus: MenuItem[], level = 0): MenuItem[] {
        let result: MenuItem[] = [];
        for (const menu of menus) {
            // We add a 'level' property for UI indentation if needed, but MenuItem doesn't have it
            // Let's just create a list for the dropdown
            result.push(menu);
            if (menu.children && menu.children.length > 0) {
                result = [...result, ...this.flattenMenus(menu.children, level + 1)];
            }
        }
        return result;
    }

    openCreateModal(parentId: string | null = null) {
        this.editingMenu.set({
            parentId: parentId || undefined,
            label: '',
            icon: '',
            routerLink: '',
            sortOrder: 0,
            requiredPermissionCode: '',
            isActive: true
        });
        this.showModal.set(true);
    }

    openEditModal(menu: MenuItem) {
        this.editingMenu.set({ ...menu });
        this.showModal.set(true);
    }

    closeModal() {
        this.showModal.set(false);
        this.editingMenu.set(null);
    }

    save() {
        const menu = this.editingMenu();
        if (!menu) return;

        if (menu.id) {
            this.menuService.updateMenu(menu.id, menu).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('menuManagement.toasts.updated'));
                    this.loadData();
                    this.closeModal();
                },
                error: () => this.toastr.error(this.translate.instant('menuManagement.toasts.updateError'))
            });
        } else {
            this.menuService.createMenu(menu).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('menuManagement.toasts.created'));
                    this.loadData();
                    this.closeModal();
                },
                error: () => this.toastr.error(this.translate.instant('menuManagement.toasts.createError'))
            });
        }
    }

    deleteMenu(id: string) {
        if (confirm(this.translate.instant('menuManagement.confirmDelete'))) {
            this.menuService.deleteMenu(id).subscribe({
                next: () => {
                    this.toastr.success(this.translate.instant('menuManagement.toasts.deleted'));
                    this.loadData();
                },
                error: (err) => this.toastr.error(err.error || this.translate.instant('menuManagement.toasts.deleteError'))
            });
        }
    }
}
