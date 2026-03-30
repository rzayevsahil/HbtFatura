import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';

import { User, AuthResponse } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'hbt_fatura_auth';
  private readonly tokenKey = 'hbt_fatura_token';
  private readonly refreshKey = 'hbt_fatura_refresh';

  private currentUser = signal<User | null>(null);
  private accessToken = signal<string | null>(null);
  private _loggingOut = signal<boolean>(false);

  user = computed(() => this.currentUser());
  isAuthenticated = computed(() => !!this.accessToken());
  loggingOut = computed(() => this._loggingOut());

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.migrateSessionAuthToLocalIfNeeded();
    const stored = localStorage.getItem(this.storageKey) ?? sessionStorage.getItem(this.storageKey);

    if (stored) {
      try {
        const data = JSON.parse(stored) as AuthResponse;
        this.currentUser.set(data.user);
        this.accessToken.set(data.accessToken);
        // Tüm sekmeler aynı oturumu görsün diye jetonlar her zaman localStorage’da tutulur
        localStorage.setItem(this.tokenKey, data.accessToken);
        localStorage.setItem(this.refreshKey, data.refreshToken);
      } catch { }
    } else {
      const t = localStorage.getItem(this.tokenKey) ?? sessionStorage.getItem(this.tokenKey);
      if (t) this.accessToken.set(t);
    }
  }

  /**
   * Eski davranış: rememberMe kapalı iken sessionStorage kullanılıyordu; bu depo sekme bazlıdır,
   * yeni sekmede oturum görünmezdi. Mevcut oturumu bir kez localStorage’a taşır.
   */
  private migrateSessionAuthToLocalIfNeeded(): void {
    if (localStorage.getItem(this.storageKey)) return;
    const sessionBlob = sessionStorage.getItem(this.storageKey);
    if (!sessionBlob) return;
    try {
      const data = JSON.parse(sessionBlob) as AuthResponse;
      localStorage.setItem(this.storageKey, sessionBlob);
      localStorage.setItem(this.tokenKey, data.accessToken);
      localStorage.setItem(this.refreshKey, data.refreshToken);
      sessionStorage.removeItem(this.storageKey);
      sessionStorage.removeItem(this.tokenKey);
      sessionStorage.removeItem(this.refreshKey);
    } catch {
      /* */
    }
  }

  getToken(): string | null {
    return this.accessToken() ?? localStorage.getItem(this.tokenKey) ?? sessionStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshKey) ?? sessionStorage.getItem(this.refreshKey);
  }

  /** Oturum her zaman localStorage’da; rememberMe yalnızca sunucuda refresh token ömrünü uzatır. */
  login(email: string, password: string, rememberMe: boolean): Observable<AuthResponse> {
    const url = '/api/auth/login';
    console.log('[AuthService] login → relative URL:', url);
    return this.http.post<AuthResponse>(url, {
      email,
      password,
      rememberMe
    }).pipe(tap((res) => this.handleAuthResponse(res)));
  }

  refreshToken(): Observable<AuthResponse | null> {
    const refresh = this.getRefreshToken();
    if (!refresh) return of(null);
    return this.http.post<AuthResponse>('/api/auth/refresh', { refreshToken: refresh }).pipe(
      tap((res) => this.handleAuthResponse(res)),
      catchError(() => {
        this.logout();
        return of(null);
      })
    );
  }

  logout(): void {
    if (this._loggingOut()) return;
    this._loggingOut.set(true);

    const refresh = this.getRefreshToken();
    if (refresh) {
      this.http.post('/api/auth/revoke', { refreshToken: refresh }).subscribe();
    }

    // Give some time for the user to see the "Logging out..." state
    setTimeout(() => {
      this.currentUser.set(null);
      this.accessToken.set(null);
      localStorage.removeItem(this.storageKey);
      localStorage.removeItem(this.tokenKey);
      localStorage.removeItem(this.refreshKey);
      sessionStorage.removeItem(this.storageKey);
      sessionStorage.removeItem(this.tokenKey);
      sessionStorage.removeItem(this.refreshKey);
      this.router.navigate(['/login']).then(() => {
        this._loggingOut.set(false);
      });
    }, 1000);
  }

  updateUser(updatedFields: Partial<User>): void {
    const current = this.currentUser();
    if (current) {
      const newUser = { ...current, ...updatedFields };
      this.currentUser.set(newUser);

      const stored = localStorage.getItem(this.storageKey) ?? sessionStorage.getItem(this.storageKey);
      if (stored) {
        try {
          const data = JSON.parse(stored) as AuthResponse;
          data.user = newUser;
          localStorage.setItem(this.storageKey, JSON.stringify(data));
          sessionStorage.removeItem(this.storageKey);
        } catch { }
      }
    }
  }

  private handleAuthResponse(res: AuthResponse): void {
    this.currentUser.set(res.user);
    this.accessToken.set(res.accessToken);
    localStorage.setItem(this.tokenKey, res.accessToken);
    localStorage.setItem(this.refreshKey, res.refreshToken);
    localStorage.setItem(this.storageKey, JSON.stringify(res));
    sessionStorage.removeItem(this.storageKey);
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.refreshKey);
  }

  hasPermission(code: string): boolean {
    const user = this.currentUser();
    if (!user) return false;
    return user.permissions.includes(code);
  }
}
