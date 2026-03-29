import { APP_INITIALIZER, ApplicationConfig, LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeTr from '@angular/common/locales/tr';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { initThemeFromStorage } from './core/theme/init-theme';

registerLocaleData(localeTr);

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: APP_INITIALIZER,
      multi: true,
      useFactory: () => () => {
        initThemeFromStorage();
        return Promise.resolve();
      }
    },
    { provide: LOCALE_ID, useValue: 'tr' },
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    provideToastr({ positionClass: 'toast-top-right', timeOut: 3000 })
  ]
};
