import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  form = this.fb.nonNullable.group({
    fullName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });
  error = '';
  loading = false;

  constructor(private fb: FormBuilder, private auth: AuthService) {}

  onSubmit(): void {
    this.error = '';
    this.loading = true;
    const v = this.form.getRawValue();
    this.auth.register(v.email, v.password, v.fullName).subscribe({
      next: () => window.location.href = '/invoices',
      error: (e) => { this.error = e.error?.message ?? 'Kayıt başarısız'; this.loading = false; },
      complete: () => { this.loading = false; }
    });
  }
}
