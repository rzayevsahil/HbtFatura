import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

/** Same-origin static files (i18n JSON, etc.); must not be sent to API base URL. */
function isLocalStaticAssetUrl(url: string): boolean {
  return url.startsWith('assets/') || url.startsWith('/assets/');
}

/** Auth endpoints: never attach refresh-on-401 (wrong password on login, avoid loops). */
function isAnonymousAuthApiRequest(url: string): boolean {
  const u = url.toLowerCase();
  return (
    u.includes('/api/auth/login') ||
    u.includes('/api/auth/register') ||
    u.includes('/api/auth/refresh') ||
    u.includes('/api/auth/revoke')
  );
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  if (!req.url.startsWith('http') && !isLocalStaticAssetUrl(req.url)) {
    const fullUrl = environment.apiUrl + req.url;
    req = req.clone({ url: fullUrl });
  }
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status !== 401) {
        return throwError(() => err);
      }

      const url = req.url;
      if (isAnonymousAuthApiRequest(url)) {
        return throwError(() => err);
      }

      const refresh = auth.getRefreshToken();
      if (!refresh) {
        auth.logout();
        return throwError(() => err);
      }

      return auth.refreshToken().pipe(
        switchMap((res) => {
          if (res?.accessToken) {
            const cloned = req.clone({
              setHeaders: { Authorization: `Bearer ${res.accessToken}` }
            });
            return next(cloned);
          }
          auth.logout();
          return throwError(() => err);
        }),
        catchError(() => {
          auth.logout();
          return throwError(() => err);
        })
      );
    })
  );
};
