import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MenuItem, MenuService } from '../../core/services/menu.service';
import { PermissionService, PermissionDto } from '../../core/services/permission.service';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'app-menu-management',
    standalone: true,
    imports: [CommonModule, FormsModule],
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
        private toastr: ToastrService
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
                this.toastr.error('Menüler yüklenemedi.');
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
                    this.toastr.success('Menü güncellendi.');
                    this.loadData();
                    this.closeModal();
                },
                error: () => this.toastr.error('Menü güncellenirken hata oluştu.')
            });
        } else {
            this.menuService.createMenu(menu).subscribe({
                next: () => {
                    this.toastr.success('Menü oluşturuldu.');
                    this.loadData();
                    this.closeModal();
                },
                error: () => this.toastr.error('Menü oluşturulurken hata oluştu.')
            });
        }
    }

    deleteMenu(id: string) {
        if (confirm('Bu menüyü silmek istediğinize emin misiniz?')) {
            this.menuService.deleteMenu(id).subscribe({
                next: () => {
                    this.toastr.success('Menü silindi.');
                    this.loadData();
                },
                error: (err) => this.toastr.error(err.error || 'Menü silinemedi.')
            });
        }
    }
}
