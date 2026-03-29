import { Injectable, signal } from '@angular/core';

const STORAGE_KEY = 'hbt-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  /** `initThemeFromStorage` sonrası DOM ile uyumlu başlangıç */
  readonly darkMode = signal(
    typeof document !== 'undefined' && document.documentElement.classList.contains('dark')
  );

  toggle(): void {
    const next = !this.darkMode();
    this.darkMode.set(next);
    document.documentElement.classList.toggle('dark', next);
    try {
      localStorage.setItem(STORAGE_KEY, next ? 'dark' : 'light');
    } catch {
      /* private mode vb. */
    }
  }
}
