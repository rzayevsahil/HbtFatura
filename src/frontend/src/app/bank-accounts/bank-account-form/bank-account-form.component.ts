import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BankAccountService } from '../../services/bank-account.service';
import { FirmService } from '../../services/firm.service';
import { FirmDto, BankAccountDto } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { IbanFormatter } from '../../core/utils/iban-formatter';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto } from '../../core/models';

@Component({
  selector: 'app-bank-account-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './bank-account-form.component.html',
  styleUrls: ['./bank-account-form.component.scss']
})
export class BankAccountFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    iban: ['', [Validators.required, Validators.minLength(32), Validators.maxLength(32)]],
    bankName: ['', Validators.required],
    currency: ['TRY', Validators.required],
    isActive: [true],
    firmId: [null as string | null]
  });
  id: string | null = null;
  editLoading = false;
  firms: FirmDto[] = [];
  currencies: LookupDto[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: BankAccountService,
    private firmApi: FirmService,
    public auth: AuthService,
    private toastr: ToastrService,
    private lookups: LookupService
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
        next: (b) => {
          this.form.patchValue({
            name: b.name,
            iban: b.iban ? IbanFormatter.format(b.iban) : '',
            bankName: b.bankName ?? '',
            currency: b.currency,
            isActive: b.isActive
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
        }
      });
    }
  }

  onIbanInput(event: Event): void {
    const formatted = IbanFormatter.handleInput(event);
    this.form.patchValue({ iban: formatted }, { emitEvent: false });
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    const v = this.form.getRawValue();
    const req: { name: string; iban?: string; bankName?: string; currency: string; firmId?: string } = { name: v.name, currency: v.currency };
    if (v.iban) req.iban = v.iban;
    if (v.bankName) req.bankName = v.bankName;
    if ('firmId' in v && v.firmId) req.firmId = v.firmId;

    if (this.id) {
      this.api.update(this.id, {
        name: v.name,
        iban: v.iban || undefined,
        bankName: v.bankName || undefined,
        currency: v.currency,
        isActive: v.isActive
      }).subscribe({
        next: () => {
          this.toastr.success('Banka hesabı güncellendi.');
          this.router.navigate(['/bank-accounts']);
        },
        error: e => {
          this.error = e.error?.message ?? 'Hata';
          this.saving = false;
          this.toastr.error(e.error?.message ?? 'Güncelleme sırasında hata oluştu.');
        },
        complete: () => { this.saving = false; }
      });
    } else {
      this.api.create(req).subscribe({
        next: () => {
          this.toastr.success('Banka hesabı oluşturuldu.');
          this.router.navigate(['/bank-accounts']);
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
