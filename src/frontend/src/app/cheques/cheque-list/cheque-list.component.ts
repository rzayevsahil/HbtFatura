import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChequeService } from '../../services/cheque.service';
import { LookupService } from '../../core/services/lookup.service';
import { ToastrService } from 'ngx-toastr';
import { ChequeOrPromissoryDto, PagedResult } from '../../core/models';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import {
  SearchableSelectComponent,
  SearchableSelectOption
} from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-cheque-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './cheque-list.component.html',
  styleUrls: ['./cheque-list.component.scss']
})
export class ChequeListComponent implements OnInit {
  items: ChequeOrPromissoryDto[] = [];
  loading = false;
  totalCount = 0;
  page = 1;
  pageSize = 20;
  filterPortfolioNumber = '';
  filterSerialNumber = '';
  filterType: number | '' = '';
  filterStatus: number | string | '' = '';
  filterDueFrom = '';
  filterDueTo = '';

  get chequeTypeFilterOptions(): SearchableSelectOption[] {
    const g = this.lookups.getGroup('ChequeType')();
    if (g.length > 0) {
      return g.map((l) => ({ id: String(l.code), primary: this.lookups.displayLookupLabel(l) }));
    }
    return [
      { id: '1', primary: this.translate.instant('chequesPage.typeCheque') },
      { id: '2', primary: this.translate.instant('chequesPage.typeNote') }
    ];
  }

  get chequeStatusFilterOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('ChequeStatus')().map((l) => ({
      id: String(l.code),
      primary: this.lookups.displayLookupLabel(l)
    }));
  }

  onChequeTypeFilterChange(v: string | null): void {
    this.filterType = v === null || v === '' ? '' : (+v as 1 | 2);
    this.applyFilter();
  }

  onChequeStatusFilterChange(v: string | null): void {
    this.filterStatus = v === null || v === '' ? '' : v;
    this.applyFilter();
  }

  constructor(
    private api: ChequeService,
    public lookups: LookupService,
    private toastr: ToastrService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) {
    this.translate.onLangChange
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.cdr.markForCheck());
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getPaged(this.page, this.pageSize, {
      type: this.filterType === '' ? undefined : this.filterType,
      status: this.filterStatus === '' ? undefined : Number(this.filterStatus),
      dueFrom: this.filterDueFrom || undefined,
      dueTo: this.filterDueTo || undefined,
      portfolioNumber: this.filterPortfolioNumber.trim() || undefined,
      serialNumber: this.filterSerialNumber.trim() || undefined
    }).subscribe({
      next: (res: PagedResult<ChequeOrPromissoryDto>) => {
        this.items = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: (e) => {
        this.loading = false;
        this.toastr.error(e.error?.message ?? this.translate.instant('chequesPage.toastListError'));
      }
    });
  }

  applyFilter(): void {
    this.page = 1;
    this.load();
  }

  delete(id: string): void {
    if (!confirm(this.translate.instant('chequesPage.deleteConfirm'))) return;
    this.api.delete(id).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('chequesPage.toastDeleted'));
        this.load();
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('chequesPage.toastDeleteFailed'))
    });
  }

  typeLabel(type: number): string {
    const fromLookup = this.lookups.getName('ChequeType', type);
    const code = String(type);
    if (fromLookup && fromLookup !== code) return fromLookup;
    if (type === 1) return this.translate.instant('chequesPage.typeCheque');
    if (type === 2) return this.translate.instant('chequesPage.typeNote');
    return code;
  }

  statusLabel(status: number): string {
    const fromLookup = this.lookups.getName('ChequeStatus', status);
    const code = String(status);
    if (fromLookup && fromLookup !== code) return fromLookup;
    const tr = this.translate.instant('chequesPage.statusCodes.' + status);
    return tr !== 'chequesPage.statusCodes.' + status ? tr : code;
  }

  /** #rrggbb + alfa (hex8) — renk yoksa nötr zemin */
  lookupBadgeBg(hex: string): string {
    const c = hex?.trim();
    if (c?.startsWith('#') && c.length >= 7) return c + '22';
    return 'rgba(148, 163, 184, 0.15)';
  }

}
