import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  if (!req.url.startsWith('http')) {
    const fullUrl = environment.apiUrl + req.url;
    console.log('[Auth Interceptor] İstek URL:', fullUrl, '| apiUrl:', environment.apiUrl);
    req = req.clone({ url: fullUrl });
  }
  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && auth.getRefreshToken()) {
        return auth.refreshToken().pipe(
          switchMap((res) => {
            if (res && res.accessToken) {
              const cloned = req.clone({
                setHeaders: { Authorization: `Bearer ${res.accessToken}` }
              });
              return next(cloned);
            }
            return throwError(() => err);
          }),
          catchError(() => throwError(() => err))
        );
      }
      return throwError(() => err);
    })
  );
};
