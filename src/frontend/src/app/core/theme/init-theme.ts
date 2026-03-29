const STORAGE_KEY = 'hbt-theme';

/** Sayfa boyamasından önce çalışır; FOUC önlenir. */
export function initThemeFromStorage(): void {
  if (typeof document === 'undefined' || typeof localStorage === 'undefined') return;
  let dark: boolean;
  try {
    const v = localStorage.getItem(STORAGE_KEY);
    if (v === 'dark') dark = true;
    else if (v === 'light') dark = false;
    else dark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  } catch {
    dark = false;
  }
  document.documentElement.classList.toggle('dark', dark);
}
