import { ChangeDetectorRef, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  CdkDrag,
  CdkDragDrop,
  CdkDropList,
  DragDropModule,
  moveItemInArray
} from '@angular/cdk/drag-drop';
import { MenuService } from '../../core/services/menu.service';
import { PermissionService } from '../../core/services/permission.service';
import { MenuItem, PermissionDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { getMenuDisplayLabel } from '../../core/i18n/menu-display-label';
import {
  buildReorderPayload,
  buildTreeFromPreorderRows,
  flattenMenuTreeWithDepth,
  isValidPreorderDepths,
  MenuRowVm
} from './menu-reorder';
import { IconPickerModalComponent } from '../../shared/icon-picker/icon-picker-modal.component';
import { MaterialIconLigaturePipe } from '../../shared/icon-picker/material-icon-ligature.pipe';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-menu-management',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    DragDropModule,
    IconPickerModalComponent,
    MaterialIconLigaturePipe,
    SearchableSelectComponent
  ],
  templateUrl: './menu-management.component.html',
  styleUrls: ['./menu-management.component.scss']
})
export class MenuManagementComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  allMenus = signal<MenuItem[]>([]);
  /** Sürükle-bırak için DFS düz liste (CDK bu diziyi günceller). */
  dragRows = signal<MenuRowVm[]>([]);
  flatMenus = signal<MenuItem[]>([]);
  permissions = signal<PermissionDto[]>([]);
  loading = signal(false);

  showModal = signal(false);
  /** Material ikon galerisi (üstteki menü modalının üzerinde, z-index:1100) */
  showIconPicker = signal(false);
  editingMenu = signal<Partial<MenuItem> | null>(null);

  readonly sortRows = (index: number, item: CdkDrag<MenuRowVm>, drop: CdkDropList<MenuRowVm[]>) => {
    const data = drop.data;
    const dragRow = item.data;
    const prevIndex = data.indexOf(dragRow);
    if (prevIndex === -1) return false;
    const copy = [...data];
    moveItemInArray(copy, prevIndex, index);
    return isValidPreorderDepths(copy);
  };

  constructor(
    private menuService: MenuService,
    private permService: PermissionService,
    private toastr: ToastrService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
  }

  ngOnInit() {
    this.loadData();
  }

  /** Tablo / üst menü seçici: bilinen rotalar ve seed etiketleri `menu.*` ile çevrilir. */
  permissionSearchableOptions(): SearchableSelectOption[] {
    return this.permissions().map(p => ({
      id: p.code,
      primary: `${p.group} - ${p.name}`,
      secondary: p.code
    }));
  }

  parentMenuSearchableOptions(): SearchableSelectOption[] {
    const cur = this.editingMenu();
    return this.flatMenus()
      .filter(m => !cur?.id || m.id !== cur.id)
      .map(m => ({
        id: m.id,
        primary: this.displayMenuLabel(m),
        secondary: m.routerLink ?? ''
      }));
  }

  displayMenuLabel(item: MenuItem): string {
    return getMenuDisplayLabel(this.translate, item);
  }

  loadData() {
    this.loading.set(true);
    this.menuService.getAllMenus().subscribe({
      next: (menus) => {
        this.allMenus.set(menus);
        this.dragRows.set(flattenMenuTreeWithDepth(menus));
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

  trackByRowId(_index: number, row: MenuRowVm): string {
    return row.menu.id;
  }

  onRowDrop(event: CdkDragDrop<MenuRowVm[]>) {
    if (event.previousContainer !== event.container) return;
    const data = event.container.data;
    moveItemInArray(data, event.previousIndex, event.currentIndex);
    if (!isValidPreorderDepths(data)) {
      moveItemInArray(data, event.currentIndex, event.previousIndex);
      this.toastr.warning(this.translate.instant('menuManagement.toasts.reorderInvalid'));
      return;
    }

    const tree = buildTreeFromPreorderRows(data);
    const payload = buildReorderPayload(tree);

    this.menuService.reorderMenus(payload).subscribe({
      next: () => {
        this.allMenus.set(tree);
        this.dragRows.set(flattenMenuTreeWithDepth(tree));
        this.flatMenus.set(this.flattenMenus(tree));
        this.menuService.fetchMenu().subscribe({ error: () => {} });
        this.toastr.success(this.translate.instant('menuManagement.toasts.reordered'));
      },
      error: () => {
        this.toastr.error(this.translate.instant('menuManagement.toasts.reorderError'));
        this.loadData();
      }
    });
  }

  flattenMenus(menus: MenuItem[], level = 0): MenuItem[] {
    let result: MenuItem[] = [];
    for (const menu of menus) {
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
    this.showIconPicker.set(false);
    this.editingMenu.set(null);
  }

  openIconPicker(): void {
    if (this.editingMenu()) {
      this.showIconPicker.set(true);
    }
  }

  onIconPicked(name: string): void {
    const m = this.editingMenu();
    if (!m) return;
    this.editingMenu.set({ ...m, icon: name });
    this.showIconPicker.set(false);
  }

  closeIconPicker(): void {
    this.showIconPicker.set(false);
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
