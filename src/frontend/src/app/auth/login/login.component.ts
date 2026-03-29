import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { LANG_STORAGE_KEY } from '../../core/i18n/app-shell.init';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslateModule],
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
  showPassword = false;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private toastr: ToastrService,
    public theme: ThemeService,
    public translate: TranslateService
  ) { }

  setLang(lang: 'tr' | 'en'): void {
    this.translate.use(lang).subscribe(() => {
      try {
        localStorage.setItem(LANG_STORAGE_KEY, lang);
      } catch {
        /* */
      }
      document.documentElement.lang = lang === 'en' ? 'en' : 'tr';
    });
  }

  isLang(lang: 'tr' | 'en'): boolean {
    return this.translate.currentLang === lang;
  }

  onSubmit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.form.getRawValue().email, this.form.getRawValue().password).subscribe({
      next: (res) => {
        window.location.href = '/dashboard';
      },
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
