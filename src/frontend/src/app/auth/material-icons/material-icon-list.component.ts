import { Component, OnInit, computed, effect, signal, untracked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { MaterialIconService } from '../../core/services/material-icon.service';
import { MaterialIconDto } from '../../core/models/material-icon.model';
import { MaterialIconLigaturePipe } from '../../shared/icon-picker/material-icon-ligature.pipe';

@Component({
  selector: 'app-material-icon-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, MaterialIconLigaturePipe],
  templateUrl: './material-icon-list.component.html',
  styleUrls: ['./material-icon-list.component.scss']
})
export class MaterialIconListComponent implements OnInit {
  private readonly pageSize = 15;
  /** Düzenlemede API’ye gönderilecek mevcut sıra (modalde gösterilmez). */
  private editSortOrder = 0;

  items = signal<MaterialIconDto[]>([]);
  loading = signal(false);
  searchQuery = signal('');
  currentPage = signal(1);

  showModal = false;
  saving = false;
  editId: string | null = null;
  model = { ligatureName: '', isActive: true };

  readonly filteredItems = computed(() => {
    const q = this.searchQuery().trim().toLowerCase();
    const list = this.items();
    if (!q) return list;
    return list.filter((x) => x.ligatureName.toLowerCase().includes(q));
  });

  readonly totalFiltered = computed(() => this.filteredItems().length);

  readonly totalPages = computed(() => {
    const n = this.filteredItems().length;
    if (n === 0) return 1;
    return Math.ceil(n / this.pageSize);
  });

  readonly pagedItems = computed(() => {
    const page = this.currentPage();
    const start = (page - 1) * this.pageSize;
    return this.filteredItems().slice(start, start + this.pageSize);
  });

  constructor(
    private api: MaterialIconService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) {
    effect(() => {
      const tp = this.totalPages();
      const cp = this.currentPage();
      if (cp > tp) {
        untracked(() => this.currentPage.set(tp));
      }
    });
  }

  ngOnInit(): void {
    this.load();
  }

  onSearchChange(value: string): void {
    this.searchQuery.set(value);
    this.currentPage.set(1);
  }

  goPrev(): void {
    this.currentPage.update((p) => Math.max(1, p - 1));
  }

  goNext(): void {
    this.currentPage.update((p) => Math.min(this.totalPages(), p + 1));
  }

  load(): void {
    this.loading.set(true);
    this.api.getAll().subscribe({
      next: (list) => {
        this.items.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.toastr.error(this.translate.instant('materialIconsPage.toasts.loadError'));
        this.loading.set(false);
      }
    });
  }

  private nextSortOrderForCreate(): number {
    const list = this.items();
    if (!list.length) return 0;
    return Math.max(...list.map((x) => x.sortOrder)) + 1;
  }

  openAdd(): void {
    this.editId = null;
    this.editSortOrder = 0;
    this.model = { ligatureName: '', isActive: true };
    this.showModal = true;
  }

  openEdit(row: MaterialIconDto): void {
    this.editId = row.id;
    this.editSortOrder = row.sortOrder;
    this.model = {
      ligatureName: row.ligatureName,
      isActive: row.isActive
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editId = null;
    this.editSortOrder = 0;
  }

  save(): void {
    const name = this.model.ligatureName?.trim().toLowerCase();
    if (!name) {
      this.toastr.warning(this.translate.instant('materialIconsPage.toasts.nameRequired'));
      return;
    }
    this.saving = true;
    const sortOrder = this.editId ? this.editSortOrder : this.nextSortOrderForCreate();
    const body = { ligatureName: name, sortOrder, isActive: this.model.isActive };
    if (this.editId) {
      this.api.update(this.editId, body).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('materialIconsPage.toasts.saved'));
          this.saving = false;
          this.closeModal();
          this.load();
        },
        error: (err) => {
          this.toastr.error(err.error ?? this.translate.instant('materialIconsPage.toasts.saveError'));
          this.saving = false;
        }
      });
    } else {
      this.api.create(body).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('materialIconsPage.toasts.saved'));
          this.saving = false;
          this.closeModal();
          this.load();
        },
        error: (err) => {
          this.toastr.error(err.error ?? this.translate.instant('materialIconsPage.toasts.saveError'));
          this.saving = false;
        }
      });
    }
  }

  delete(row: MaterialIconDto): void {
    if (!confirm(this.translate.instant('materialIconsPage.confirmDelete', { name: row.ligatureName }))) {
      return;
    }
    this.api.delete(row.id).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('materialIconsPage.toasts.deleted'));
        this.load();
      },
      error: () => this.toastr.error(this.translate.instant('materialIconsPage.toasts.deleteError'))
    });
  }
}
