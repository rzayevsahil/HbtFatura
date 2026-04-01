import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom, LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import localeEn from '@angular/common/locales/en';
import { provideRouter } from '@angular/router';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { initAppShell } from './core/i18n/app-shell.init';
import { createDbTranslateLoader } from './core/i18n/db-translate.loader';

registerLocaleData(localeTr);
registerLocaleData(localeEn);

export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'tr',
        loader: {
          provide: TranslateLoader,
          useFactory: createDbTranslateLoader,
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
