import { HttpClient } from '@angular/common/http';
import { TranslateLoader } from '@ngx-translate/core';
import { Observable, catchError, forkJoin, map, of, shareReplay } from 'rxjs';

function isPlainObject(v: unknown): v is Record<string, unknown> {
  return v !== null && typeof v === 'object' && !Array.isArray(v);
}

/** Statik JSON + API (DB); API aynı yolu günceller, eksik yeni anahtarlar assets’ten gelir. */
function deepMerge(
  base: Record<string, unknown>,
  over: Record<string, unknown>
): Record<string, unknown> {
  const result: Record<string, unknown> = { ...base };
  for (const key of Object.keys(over)) {
    const b = base[key];
    const o = over[key];
    if (isPlainObject(b) && isPlainObject(o)) {
      result[key] = deepMerge(b, o);
    } else {
      result[key] = o;
    }
  }
  return result;
}

/**
 * Dil başına tek istek çifti + birleştirme; sonuç oturum boyunca paylaşılır (tekrar yükleme yok).
 */
export class DbTranslateLoader implements TranslateLoader {
  private static readonly mergedCache = new Map<string, Observable<Record<string, unknown>>>();

  constructor(private readonly http: HttpClient) {}

  getTranslation(lang: string): Observable<Record<string, unknown>> {
    const hit = DbTranslateLoader.mergedCache.get(lang);
    if (hit) {
      return hit;
    }

    const assets$ = this.http.get<Record<string, unknown>>(`assets/i18n/${lang}.json`).pipe(
      catchError(() => of({} as Record<string, unknown>))
    );
    const api$ = this.http.get<Record<string, unknown>>(`/api/translations/${lang}`).pipe(
      catchError(() => of({} as Record<string, unknown>))
    );

    const merged$ = forkJoin({ assets: assets$, api: api$ }).pipe(
      map(({ assets, api }) => deepMerge(assets, api)),
      shareReplay({ bufferSize: 1, refCount: false })
    );

    DbTranslateLoader.mergedCache.set(lang, merged$);
    return merged$;
  }
}

export function createDbTranslateLoader(http: HttpClient): TranslateLoader {
  return new DbTranslateLoader(http);
}
