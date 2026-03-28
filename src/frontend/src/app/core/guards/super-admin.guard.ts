import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Yalnızca SuperAdmin erişebilir (ör. firma listesi / yeni firma). Diğer roller → faturalar. */
export const superAdminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated() && auth.user()?.role === 'SuperAdmin') {
    return true;
  }

  router.navigate(['/invoices']);
  return false;
};

/**
 * SuperAdmin bu rotalara giremez (ör. personel yönetimi); firma detayındaki kullanıcı listesi ayrı API ile kalır.
 * Kimliği doğrulanmamışsa → login.
 */
export const notSuperAdminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  if (auth.user()?.role === 'SuperAdmin') {
    router.navigate(['/firms']);
    return false;
  }

  return true;
};
