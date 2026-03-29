import { TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { initThemeFromStorage } from '../theme/init-theme';

export const LANG_STORAGE_KEY = 'hbt-lang';

export function initAppShell(translate: TranslateService): () => Promise<unknown> {
  return () => {
    initThemeFromStorage();
    const lang = localStorage.getItem(LANG_STORAGE_KEY) === 'en' ? 'en' : 'tr';
    translate.addLangs(['tr', 'en']);
    translate.setDefaultLang('tr');
    document.documentElement.lang = lang === 'en' ? 'en' : 'tr';
    return firstValueFrom(translate.use(lang));
  };
}
