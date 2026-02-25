import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BankAccountService } from '../../services/bank-account.service';
import { FirmService, FirmDto } from '../../services/firm.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

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
    iban: [''],
    bankName: [''],
    currency: ['TRY'],
    isActive: [true],
    firmId: [null as string | null]
  });
  id: string | null = null;
  firms: FirmDto[] = [];
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private api: BankAccountService,
    private firmApi: FirmService,
    public auth: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.auth.user()?.role === 'SuperAdmin') {
      this.firmApi.getAll().subscribe(list => this.firms = list);
    } else {
      (this.form as FormGroup).removeControl('firmId');
    }
    if (this.id) {
      this.api.getById(this.id).subscribe(b => this.form.patchValue({
        name: b.name,
        iban: b.iban ?? '',
        bankName: b.bankName ?? '',
        currency: b.currency,
        isActive: b.isActive
      }));
    }
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
      this.api.update(this.id, { name: v.name, iban: v.iban || undefined, bankName: v.bankName || undefined, isActive: v.isActive }).subscribe({
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
