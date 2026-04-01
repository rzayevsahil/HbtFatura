import { HttpClient } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { Observable, catchError } from 'rxjs';

/** Önce API’den yükler; hata olursa statik assets/i18n yedeğine düşer. */
export class DbTranslateLoader implements TranslateLoader {
  constructor(private readonly http: HttpClient) {}

  getTranslation(lang: string): Observable<Record<string, unknown>> {
    return this.http.get<Record<string, unknown>>(`/api/translations/${lang}`).pipe(
      catchError(() => this.http.get<Record<string, unknown>>(`assets/i18n/${lang}.json`))
    );
  }
}

export function createDbTranslateLoader(http: HttpClient): TranslateLoader {
  return new DbTranslateLoader(http);
}
