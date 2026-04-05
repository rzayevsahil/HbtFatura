import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ChequeService } from '../../services/cheque.service';
import { LookupService } from '../../core/services/lookup.service';
import { ChequeOrPromissoryDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-cheque-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule, SearchableSelectComponent],
  templateUrl: './cheque-detail.component.html',
  styleUrls: ['./cheque-detail.component.scss']
})
export class ChequeDetailComponent implements OnInit {
  item: ChequeOrPromissoryDto | null = null;
  loading = true;
  newStatus: number = 0;

  get chequeStatusSearchableOptions(): SearchableSelectOption[] {
    return this.lookups.getGroup('ChequeStatus')().map((l) => ({
      id: String(l.code),
      primary: this.lookups.displayLookupLabel(l)
    }));
  }

  constructor(
    private route: ActivatedRoute,
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
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.api.getById(id).subscribe({
        next: (c) => {
          this.item = c;
          this.newStatus = c.status;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    } else {
      this.loading = false;
    }
  }

  updateStatus(): void {
    if (!this.item) return;
    this.api.setStatus(this.item.id, this.newStatus).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('chequesPage.toastStatusSaved'));
        this.item = { ...this.item!, status: this.newStatus };
      },
      error: e => this.toastr.error(e.error?.message ?? this.translate.instant('chequesPage.toastStatusError'))
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

  lookupBadgeBg(hex: string): string {
    const c = hex?.trim();
    if (c?.startsWith('#') && c.length >= 7) return c + '22';
    return 'rgba(148, 163, 184, 0.15)';
  }

}
