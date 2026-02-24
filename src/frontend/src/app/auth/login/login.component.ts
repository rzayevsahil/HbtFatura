import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });
  error = '';
  loading = false;

  constructor(private fb: FormBuilder, private auth: AuthService, private toastr: ToastrService) {}

  onSubmit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.form.getRawValue().email, this.form.getRawValue().password).subscribe({
      next: () => window.location.href = '/invoices',
      error: (e) => {
        const msg = e.error?.message
          ?? (e.status === 0 ? 'API\'ye bağlanılamadı. Backend çalışıyor mu?' : 'E-posta veya şifre hatalı.');
        this.error = msg;
        this.loading = false;
        this.toastr.error(msg);
      },
      complete: () => { this.loading = false; }
    });
  }
}
