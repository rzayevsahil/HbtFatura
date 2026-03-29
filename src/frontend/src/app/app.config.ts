import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom, LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import localeEn from '@angular/common/locales/en';
import { provideRouter } from '@angular/router';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { initAppShell } from './core/i18n/app-shell.init';

registerLocaleData(localeTr);
registerLocaleData(localeEn);

export function createTranslateLoader(http: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(http, 'assets/i18n/', '.json');
}

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'tr',
        loader: {
          provide: TranslateLoader,
          useFactory: createTranslateLoader,
          deps: [HttpClient]
        }
      })
    ),
    {
      provide: APP_INITIALIZER,
      multi: true,
      useFactory: initAppShell,
      deps: [TranslateService]
    },
    { provide: LOCALE_ID, useValue: 'tr' },
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    provideToastr({ positionClass: 'toast-top-right', timeOut: 3000 })
  ]
};
