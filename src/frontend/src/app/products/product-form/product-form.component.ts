import { ChangeDetectorRef, Component, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { merge, of } from 'rxjs';
import { catchError, debounceTime, map, startWith, switchMap } from 'rxjs/operators';
import { ProductService } from '../../services/product.service';
import { AuthService } from '../../core/services/auth.service';
import { FirmService } from '../../services/firm.service';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto, ProductDto } from '../../core/models';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, SearchableSelectComponent, TranslateModule],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    barcode: [''],
    stockType: ['ticari mal', Validators.required],
    unit: ['Adet'],
    stockQuantity: [0],
    unitPrice: [0],
    stockDiscountPercent: [0, [Validators.min(0), Validators.max(100)]],
    stockDiscountAmount: [0, [Validators.min(0)]],
    currency: ['TRY'],
    firmId: [null as string | null]
  });
  id: string | null = null;
  editLoading = false;
  /** Düzenlemede kod kontrolü için ürünün firması */
  private firmIdForScope: string | null = null;
  firms: { id: string; name: string }[] = [];
  currencies: LookupDto[] = [];
  stockTypes: LookupDto[] = [];
  error = '';
  saving = false;
  /** Yeni ürün: sipariş kalemi benzeri öneri listesi (firma kapsamı) */
  productsForSuggest: ProductDto[] = [];
  activeNewProductSuggest: 'code' | 'name' | null = null;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: ProductService,
    public auth: AuthService,
    private firmApi: FirmService,
    private toastr: ToastrService,
    public lookups: LookupService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) { }

  get firmSearchableOptions(): SearchableSelectOption[] {
    return this.firms.map(f => ({ id: f.id, primary: f.name }));
  }

  get stockTypeSearchableOptions(): SearchableSelectOption[] {
    return this.stockTypes.map(t => ({
      id: t.code,
      primary: this.translate.instant('products.stockTypes.' + t.code) || t.name,
    }));
  }

  get currencySearchableOptions(): SearchableSelectOption[] {
    return this.currencies.map(c => ({
      id: c.code,
      primary: `${c.name}`,
      secondary: c.code
    }));
  }

  /** Formdaki `unit` değeri lookup’ta `name || code` ile tutulur (eski UnitFieldSelect ile aynı). */
  get unitSearchableOptions(): SearchableSelectOption[] {
    const list = this.lookups.getGroup('ProductUnit')();
    const opts: SearchableSelectOption[] = list.map(l => {
      const id = ((l.name || l.code || '').trim() || 'Adet');
      return {
        id,
        primary: this.lookups.displayProductUnitLabel(l) || id,
      };
    });
    const current = (this.form.get('unit')?.value ?? '').toString().trim();
    if (current && !opts.some(o => o.id === current)) {
      opts.unshift({
        id: current,
        primary: this.lookups.getName('ProductUnit', current) || current,
      });
    }
    return opts;
  }

  ngOnInit(): void {
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(f => (this.firms = f));
    }
    this.lookups.load().subscribe(list => {
      this.currencies = list.filter(x => x.group?.name === 'Currency' && x.isActive);
      this.stockTypes = list.filter(x => x.group?.name === 'ProductStockType' && x.isActive);
    });
    this.id = this.route.snapshot.paramMap.get('id');

    merge(
      this.form.controls.code.valueChanges.pipe(startWith(this.form.controls.code.getRawValue())),
      this.form.controls.firmId.valueChanges.pipe(startWith(this.form.controls.firmId.getRawValue()))
    ).pipe(
      debounceTime(400),
      switchMap(() => {
        const code = (this.form.controls.code.getRawValue() ?? '').toString().trim();
        const firmId = this.getFirmIdForCodeCheck();
        if (!code || !firmId) return of(false);
        return this.api.isCodeTaken(code, firmId, this.id ?? undefined).pipe(
          map(r => r.taken),
          catchError(() => of(false))
        );
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(taken => this.setDuplicateCodeError(taken));

    if (!this.id) {
      this.refreshProductSuggestions();
      if (this.auth.user()?.role === 'SuperAdmin') {
        this.form.controls.firmId.valueChanges
          .pipe(startWith(this.form.controls.firmId.getRawValue()), takeUntilDestroyed(this.destroyRef))
          .subscribe(() => this.refreshProductSuggestions());
      }
    }

    if (this.id) {
      this.editLoading = true;
      this.api.getById(this.id).subscribe({
        next: (p) => {
          this.firmIdForScope = p.firmId;
          this.form.patchValue({
            code: p.code,
            name: p.name,
            barcode: p.barcode ?? '',
            stockType: p.stockType ?? 'ticari mal',
            unit: p.unit ?? 'Adet',
            stockQuantity: p.stockQuantity ?? 0,
            unitPrice: p.unitPrice ?? 0,
            stockDiscountPercent: p.stockDiscountPercent ?? 0,
            stockDiscountAmount: p.stockDiscountAmount ?? 0,
            currency: p.currency ?? 'TRY'
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
        }
      });
    }
  }

  private refreshProductSuggestions(): void {
    if (this.id) return;
    const firmId = this.getFirmIdForCodeCheck();
    if (!firmId) {
      this.productsForSuggest = [];
      return;
    }
    this.api.getDropdown(firmId).subscribe({
      next: list => (this.productsForSuggest = list),
      error: () => (this.productsForSuggest = [])
    });
  }

  getFilteredNewProductSuggestions(searchText: string | null | undefined): ProductDto[] {
    const t = (searchText ?? '').trim().toLowerCase();
    if (!t) return [];
    return this.productsForSuggest
      .filter(
        p =>
          (p.code?.toLowerCase().includes(t)) ||
          (p.name?.toLowerCase().includes(t)) ||
          (p.barcode != null && p.barcode.toLowerCase().includes(t))
      )
      .slice(0, 12);
  }

  onNewProductFieldFocus(field: 'code' | 'name'): void {
    if (this.id) return;
    this.activeNewProductSuggest = field;
  }

  onNewProductFieldInput(field: 'code' | 'name'): void {
    if (this.id) return;
    this.activeNewProductSuggest = field;
  }

  onNewProductSuggestSelect(p: ProductDto): void {
    if (this.id) return;
    const disc = p as unknown as { stockDiscountPercent?: number; stockDiscountAmount?: number };
    this.form.patchValue({
      code: p.code,
      name: p.name,
      barcode: p.barcode ?? '',
      stockType: p.stockType ?? 'ticari mal',
      unit: (p.unit || 'Adet').trim() || 'Adet',
      stockQuantity: 0,
      unitPrice: p.unitPrice ?? 0,
      stockDiscountPercent: disc.stockDiscountPercent ?? 0,
      stockDiscountAmount: disc.stockDiscountAmount ?? 0,
      currency: (p.currency as string) || 'TRY'
    });
    this.activeNewProductSuggest = null;
    this.toastr.info(this.translate.instant('products.suggestFilledFromExisting'));
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    const el = e.target as HTMLElement;
    if (!el.closest('.autocomplete-container')) {
      this.activeNewProductSuggest = null;
    }
  }

  private getFirmIdForCodeCheck(): string | null {
    if (this.id && this.firmIdForScope) return this.firmIdForScope;
    if (this.auth.user()?.role === 'SuperAdmin') {
      const v = this.form.controls.firmId.getRawValue();
      return v ? String(v) : null;
    }
    const fid = this.auth.user()?.firmId;
    return fid ? String(fid) : null;
  }

  private setDuplicateCodeError(taken: boolean): void {
    const c = this.form.controls.code;
    const errs = { ...(c.errors ?? {}) } as Record<string, unknown>;
    if (taken) errs['duplicateCode'] = true;
    else delete errs['duplicateCode'];
    c.setErrors(Object.keys(errs).length ? errs : null);
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const payload = {
      code: v.code,
      name: v.name,
      barcode: v.barcode || undefined,
      stockType: (v.stockType || 'ticari mal'),
      unit: v.unit || 'Adet',
      stockQuantity: v.stockQuantity ?? 0,
      unitPrice: v.unitPrice ?? 0,
      stockDiscountPercent: v.stockDiscountPercent ?? 0,
      stockDiscountAmount: v.stockDiscountAmount ?? 0,
      currency: (v.currency as any) || 'TRY',
      firmId: (this.auth.user()?.role === 'SuperAdmin' && v.firmId) ? v.firmId : undefined
    };
    if (this.id) {
      this.api.update(this.id, payload).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('products.toastUpdated'));
          this.router.navigate(['/products']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('products.toastUpdateError'));
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(payload).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('products.toastCreated'));
          this.router.navigate(['/products']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('products.toastSaveError'));
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
