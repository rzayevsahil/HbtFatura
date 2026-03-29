import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { ToastrService } from 'ngx-toastr';
import { sanitizeInternalReturnUrl } from '../../core/utils/sanitize-return-url';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule],
  templateUrl: './employee-form.component.html',
  styleUrls: ['./employee-form.component.scss']
})
export class EmployeeFormComponent implements OnInit {
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    fullName: ['', Validators.required]
  });
  id: string | null = null;
  editLoading = false;
  error = '';
  saving = false;
  /** Firma detayından gelindiyse iptal ve bazı yönlendirmeler için */
  cancelUrl = '/employees';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private api: EmployeeService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    const q = this.route.snapshot.queryParamMap.get('returnUrl');
    const safe = sanitizeInternalReturnUrl(q);
    if (safe) this.cancelUrl = safe;

    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.editLoading = true;
      // Düzenleme modunda şifre zorunlu olmasın
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.setValidators([Validators.minLength(6)]);
      this.form.get('password')?.updateValueAndValidity();

      this.api.getById(this.id).subscribe({
        next: emp => {
          this.form.patchValue({
            email: emp.email,
            fullName: emp.fullName
          });
          this.editLoading = false;
        },
        error: () => {
          this.editLoading = false;
          this.toastr.error(this.translate.instant('employeesPage.toastLoadError'));
        }
      });
    }
  }

  onSubmit(): void {
    this.error = '';
    this.saving = true;

    const val = this.form.getRawValue();
    const obs = this.id
      ? this.api.update(this.id, val)
      : this.api.create(val);

    obs.subscribe({
      next: (res: any) => {
        this.toastr.success(
          this.id ? this.translate.instant('employeesPage.toastUpdated') : this.translate.instant('employeesPage.toastCreated')
        );
        if (res.firmId) {
          this.router.navigate(['/firms', res.firmId]);
        } else {
          this.router.navigateByUrl(this.cancelUrl);
        }
      },
      error: (e: any) => {
        this.error = e.error?.message ?? 'Hata';
        this.saving = false;
        this.toastr.error(e.error?.message ?? this.translate.instant('employeesPage.toastSaveError'));
      },
      complete: () => { this.saving = false; }
    });
  }
}
