import { Component, DestroyRef, inject, OnInit } from '@angular/core';
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
import { LookupDto } from '../../core/models';
import { UnitFieldSelectComponent } from '../../shared/unit-field-select/unit-field-select.component';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, UnitFieldSelectComponent],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  form = this.fb.nonNullable.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    barcode: [''],
    unit: ['Adet'],
    stockQuantity: [0],
    unitPrice: [0],
    currency: ['TRY'],
    firmId: [null as string | null]
  });
  id: string | null = null;
  /** Düzenlemede kod kontrolü için ürünün firması */
  private firmIdForScope: string | null = null;
  firms: { id: string; name: string }[] = [];
  currencies: LookupDto[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: ProductService,
    public auth: AuthService,
    private firmApi: FirmService,
    private toastr: ToastrService,
    public lookups: LookupService
  ) { }

  ngOnInit(): void {
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(f => (this.firms = f));
    }
    this.lookups.load().subscribe(list => {
      this.currencies = list.filter(x => x.group?.name === 'Currency' && x.isActive);
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

    if (this.id) {
      this.api.getById(this.id).subscribe(p => {
        this.firmIdForScope = p.firmId;
        this.form.patchValue({
          code: p.code,
          name: p.name,
          barcode: p.barcode ?? '',
          unit: p.unit ?? 'Adet',
          stockQuantity: p.stockQuantity ?? 0,
          unitPrice: p.unitPrice ?? 0,
          currency: p.currency ?? 'TRY'
        });
      });
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
      unit: v.unit || 'Adet',
      stockQuantity: v.stockQuantity ?? 0,
      unitPrice: v.unitPrice ?? 0,
      currency: (v.currency as any) || 'TRY',
      firmId: (this.auth.user()?.role === 'SuperAdmin' && v.firmId) ? v.firmId : undefined
    };
    if (this.id) {
      this.api.update(this.id, payload).subscribe({
        next: () => {
          this.toastr.success('Ürün güncellendi.');
          this.router.navigate(['/products']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(payload).subscribe({
        next: () => {
          this.toastr.success('Ürün eklendi.');
          this.router.navigate(['/products']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
