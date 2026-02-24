import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * SuperAdmin: Sadece Firmalar → Detay → "Firma bilgilerini düzenle" ile (yani ?firmId= ile) girebilir; menüde link yok.
 * FirmAdmin ve Employee: Her zaman /company erişebilir (menüden "Firma Bilgileri").
 */
export const companyGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const role = auth.user()?.role;
  if (role === 'SuperAdmin') {
    const firmId = route.queryParamMap.get('firmId');
    if (!firmId) {
      router.navigate(['/firms']);
      return false;
    }
  }
  return true;
};
