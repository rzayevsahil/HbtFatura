import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
  firmId: string | null;
  firmName: string | null;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly storageKey = 'hbt_fatura_auth';
  private readonly tokenKey = 'hbt_fatura_token';
  private readonly refreshKey = 'hbt_fatura_refresh';

  private currentUser = signal<User | null>(null);
  private accessToken = signal<string | null>(null);

  user = computed(() => this.currentUser());
  isAuthenticated = computed(() => !!this.accessToken());

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    const stored = localStorage.getItem(this.storageKey);
    if (stored) {
      try {
        const data = JSON.parse(stored) as AuthResponse;
        this.currentUser.set(data.user);
        this.accessToken.set(data.accessToken);
        localStorage.setItem(this.tokenKey, data.accessToken);
        localStorage.setItem(this.refreshKey, data.refreshToken);
      } catch { }
    } else {
      const t = localStorage.getItem(this.tokenKey);
      if (t) this.accessToken.set(t);
    }
  }

  getToken(): string | null {
    return this.accessToken() ?? localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshKey);
  }

  login(email: string, password: string): Observable<AuthResponse> {
    const url = '/api/auth/login';
    console.log('[AuthService] login → relative URL:', url);
    return this.http.post<AuthResponse>(url, {
      email,
      password
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
    const refresh = this.getRefreshToken();
    if (refresh) {
      this.http.post('/api/auth/revoke', { refreshToken: refresh }).subscribe();
    }
    this.currentUser.set(null);
    this.accessToken.set(null);
    localStorage.removeItem(this.storageKey);
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshKey);
    this.router.navigate(['/login']);
  }

  updateUser(updatedFields: Partial<User>): void {
    const current = this.currentUser();
    if (current) {
      const newUser = { ...current, ...updatedFields };
      this.currentUser.set(newUser);

      const stored = localStorage.getItem(this.storageKey);
      if (stored) {
        try {
          const data = JSON.parse(stored) as AuthResponse;
          data.user = newUser;
          localStorage.setItem(this.storageKey, JSON.stringify(data));
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
  }
}
