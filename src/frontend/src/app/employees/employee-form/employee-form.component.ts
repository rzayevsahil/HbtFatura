import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
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
export class EmployeeFormComponent {
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    fullName: ['', Validators.required]
  });
  error = '';
  saving = false;

  constructor(private fb: FormBuilder, private router: Router, private api: EmployeeService, private toastr: ToastrService) {}

  onSubmit(): void {
    this.error = '';
    this.saving = true;
    this.api.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.toastr.success('Çalışan eklendi.');
        this.router.navigate(['/employees']);
      },
      error: e => {
        this.error = e.error?.message ?? 'Hata';
        this.saving = false;
        this.toastr.error(e.error?.message ?? 'Çalışan eklenirken hata oluştu.');
      },
      complete: () => { this.saving = false; }
    });
  }
}
