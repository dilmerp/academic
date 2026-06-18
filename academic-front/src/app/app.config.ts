import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeng/themes/aura';
import { routes } from './app.routes';

// Importamos el interceptor que inyectará el Token JWT
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),

    // Mantenemos withFetch() por rendimiento, y agregamos withInterceptors() por seguridad
    provideHttpClient(
      withFetch(),
      withInterceptors([authInterceptor])
    ),

    provideAnimationsAsync(),
    providePrimeNG({
        theme: { preset: Aura }
    })
  ]
};
