import { Component, OnInit, computed } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CompanyService } from '../../services/company.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-company-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './company-settings.component.html',
  styleUrls: ['./company-settings.component.scss']
})
export class CompanySettingsComponent implements OnInit {
  isReadOnly = computed(() => this.auth.user()?.role === 'Employee');
  firmId: string | undefined;

  form = this.fb.nonNullable.group({
    companyName: ['', Validators.required],
    taxOffice: [''],
    taxNumber: [''],
    address: [''],
    phone: [''],
    email: [''],
    iban: [''],
    bankName: ['']
  });
  error = '';
  saving = false;
  loading = true;

  constructor(
    private fb: FormBuilder,
    private api: CompanyService,
    public auth: AuthService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.firmId = this.route.snapshot.queryParamMap.get('firmId') ?? undefined;
    this.loading = true;
    this.api.get(this.firmId).subscribe({
      next: c => {
        this.form.patchValue({
          companyName: c.companyName,
          taxOffice: c.taxOffice ?? '',
          taxNumber: c.taxNumber ?? '',
          address: c.address ?? '',
          phone: c.phone ?? '',
          email: c.email ?? '',
          iban: c.iban ?? '',
          bankName: c.bankName ?? ''
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false; // 404 = no settings yet
      }
    });
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    this.api.save(this.form.getRawValue(), this.firmId).subscribe({
      next: () => {
        this.saving = false;
        this.toastr.success('Firma bilgileri kaydedildi.');
        this.location.back();
      },
      error: e => {
        this.error = e.error?.message ?? 'Hata';
        this.saving = false;
        this.toastr.error(e.error?.message ?? 'Kayıt sırasında hata oluştu.');
      },
      complete: () => { this.saving = false; }
    });
  }

  goBack(): void {
    this.location.back();
  }
}
