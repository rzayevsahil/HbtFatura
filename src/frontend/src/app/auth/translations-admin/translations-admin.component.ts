import { ApplicationRef, Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import {
  UiTranslationPairAdminDto,
  UiTranslationAdminService
} from '../../core/services/ui-translation-admin.service';

@Component({
  selector: 'app-translations-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './translations-admin.component.html',
  styleUrls: ['./translations-admin.component.scss']
})
export class TranslationsAdminComponent implements OnInit {
  private readonly pageSize = 40;
  private readonly appRef = inject(ApplicationRef);

  items = signal<UiTranslationPairAdminDto[]>([]);
  totalRows = signal(0);
  loading = signal(false);
  searchQuery = signal('');
  currentPage = signal(1);
  jumpPageInput = '1';

  showModal = false;
  saving = false;
  editRow: UiTranslationPairAdminDto | null = null;
  editValueTr = '';
  editValueEn = '';

  readonly totalPages = computed(() => {
    const n = this.totalRows();
    if (n === 0) return 1;
    return Math.ceil(n / this.pageSize);
  });

  constructor(
    private readonly api: UiTranslationAdminService,
    private readonly toastr: ToastrService,
    private readonly translate: TranslateService
  ) {
    effect(() => {
      const tp = this.totalPages();
      const cp = this.currentPage();
      if (cp > tp) {
        untracked(() => this.currentPage.set(tp));
      }
    });

    effect(() => {
      const cp = this.currentPage();
      untracked(() => {
        this.jumpPageInput = String(cp);
      });
    });
  }

  ngOnInit(): void {
    this.load();
  }

  onSearchChange(value: string): void {
    this.searchQuery.set(value);
    this.currentPage.set(1);
    this.load();
  }

  goPrev(): void {
    this.currentPage.update((p) => Math.max(1, p - 1));
    queueMicrotask(() => this.load());
  }

  goNext(): void {
    this.currentPage.update((p) => Math.min(this.totalPages(), p + 1));
    queueMicrotask(() => this.load());
  }

  onJumpKeydown(ev: KeyboardEvent): void {
    if (ev.key !== 'Enter') return;
    ev.preventDefault();
    this.commitJumpPage();
    (ev.target as HTMLInputElement | null)?.blur();
  }

  commitJumpPage(): void {
    const max = this.totalPages();
    const raw = String(this.jumpPageInput ?? '').trim();
    if (raw === '') {
      this.jumpPageInput = String(this.currentPage());
      return;
    }
    const n = parseInt(raw, 10);
    if (Number.isNaN(n)) {
      this.jumpPageInput = String(this.currentPage());
      return;
    }
    const page = Math.max(1, Math.min(max, n));
    this.currentPage.set(page);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    const skip = (this.currentPage() - 1) * this.pageSize;
    this.api
      .listPairs({
        q: this.searchQuery().trim() || undefined,
        skip,
        take: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.items.set(res.items ?? []);
          this.totalRows.set(res.total ?? 0);
          this.loading.set(false);
        },
        error: () => {
          this.toastr.error(this.translate.instant('translationsPage.toasts.loadError'));
          this.loading.set(false);
        }
      });
  }

  openEdit(row: UiTranslationPairAdminDto): void {
    this.editRow = row;
    this.editValueTr = row.valueTr ?? '';
    this.editValueEn = row.valueEn ?? '';
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editRow = null;
    this.editValueTr = '';
    this.editValueEn = '';
  }

  save(): void {
    if (!this.editRow) return;
    this.saving = true;
    this.api.updatePair(this.editRow.key, this.editValueTr, this.editValueEn).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('translationsPage.toasts.saved'));
        this.saving = false;
        this.closeModal();
        this.load();
        forkJoin([this.translate.reloadLang('tr'), this.translate.reloadLang('en')]).subscribe(() =>
          this.appRef.tick()
        );
      },
      error: () => {
        this.toastr.error(this.translate.instant('translationsPage.toasts.saveError'));
        this.saving = false;
      }
    });
  }
}
