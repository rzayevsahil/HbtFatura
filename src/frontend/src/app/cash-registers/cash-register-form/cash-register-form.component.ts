import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CashRegisterService } from '../../services/cash-register.service';
import { FirmService } from '../../services/firm.service';
import { FirmDto, CashRegisterDto } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto } from '../../core/models';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SearchableSelectComponent, SearchableSelectOption } from '../../shared/searchable-select/searchable-select.component';

@Component({
  selector: 'app-cash-register-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule, SearchableSelectComponent],
  templateUrl: './cash-register-form.component.html',
  styleUrls: ['./cash-register-form.component.scss']
})
export class CashRegisterFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    currency: ['TRY'],
    isActive: [true],
    firmId: [null as string | null]
  });
  id: string | null = null;
  editLoading = false;
  firms: FirmDto[] = [];
  currencies: LookupDto[] = [];
  error = '';
  saving = false;

  get firmSearchableOptions(): SearchableSelectOption[] {
    return this.firms.map(f => ({ id: f.id, primary: f.name }));
  }

  get currencySearchableOptions(): SearchableSelectOption[] {
    return this.currencies.map(c => ({
      id: c.code,
      primary: `${c.name}`,
      secondary: c.code
    }));
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: CashRegisterService,
    private firmApi: FirmService,
    public auth: AuthService,
    private toastr: ToastrService,
    private lookups: LookupService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.lookups.load().subscribe(list => {
      this.currencies = list.filter(x => x.group?.name === 'Currency' && x.isActive);
    });
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(list => this.firms = list);
    } else {
      (this.form as FormGroup).removeControl('firmId');
    }
    if (this.id) {
      this.editLoading = true;
      this.api.getById(this.id).subscribe({
        next: (c) => {
          this.form.patchValue({
            name: c.name,
            currency: c.currency,
            isActive: c.isActive
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
        }
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const req: { name: string; currency: string; firmId?: string } = { name: v.name, currency: v.currency };
    if ('firmId' in v && v.firmId) req.firmId = v.firmId;

    if (this.id) {
      this.api.update(this.id, { name: v.name, currency: v.currency, isActive: v.isActive }).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('cashRegisters.toastUpdated'));
          this.router.navigate(['/cash-registers']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('cashRegisters.updateError'));
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(req).subscribe({
        next: () => {
          this.toastr.success(this.translate.instant('cashRegisters.toastCreated'));
          this.router.navigate(['/cash-registers']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? this.translate.instant('cashRegisters.saveError'));
        },
        complete: () => { this.saving = false; }
      });
    }
  }
}
