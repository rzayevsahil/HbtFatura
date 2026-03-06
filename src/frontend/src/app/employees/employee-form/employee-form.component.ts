import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EmployeeService } from '../../services/employee.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
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
  error = '';
  saving = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private api: EmployeeService,
    private toastr: ToastrService
  ) { }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
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
        },
        error: () => this.toastr.error('Çalışan bilgileri yüklenemedi.')
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
      next: () => {
        this.toastr.success(this.id ? 'Çalışan güncellendi.' : 'Çalışan eklendi.');
        this.router.navigate(['/employees']);
      },
      error: (e: any) => {
        this.error = e.error?.message ?? 'Hata';
        this.saving = false;
        this.toastr.error(e.error?.message ?? 'İşlem sırasında hata oluştu.');
      },
      complete: () => { this.saving = false; }
    });
  }
}
