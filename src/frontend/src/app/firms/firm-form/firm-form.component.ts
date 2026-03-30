import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FirmService } from '../../services/firm.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-firm-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule],
  templateUrl: './firm-form.component.html',
  styleUrls: ['./firm-form.component.scss']
})
export class FirmFormComponent {
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    adminEmail: ['', [Validators.required, Validators.email]],
    adminPassword: ['', [Validators.required, Validators.minLength(6)]],
    adminFullName: ['', Validators.required]
  });
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private api: FirmService,
    private toastr: ToastrService,
    private translate: TranslateService
  ) {}

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    this.api.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('firmForm.toasts.created'));
        this.router.navigate(['/firms']);
      },
      error: e => {
        this.error = e.error?.message ?? this.translate.instant('firmForm.errorFallback');
        this.saving = false;
        this.toastr.error(e.error?.message ?? this.translate.instant('firmForm.toasts.createError'));
      },
      complete: () => { this.saving = false; }
    });
  }
}
