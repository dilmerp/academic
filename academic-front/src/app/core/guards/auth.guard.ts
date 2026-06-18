import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Verificamos si el usuario tiene un token válido en la sesión
  if (authService.isAuthenticated()) {
    return true; // Permite el acceso a la ruta solicitada
  }

  // Si no está autenticado, lo redirigimos forzosamente al Login
  router.navigate(['/login']);
  return false; // Bloquea el acceso a la ruta protegida
};
