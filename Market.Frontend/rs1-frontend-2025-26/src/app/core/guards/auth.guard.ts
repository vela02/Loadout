import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthFacadeService } from '../../feature-services/auth/auth-facade.service';

/**
 * Guard that checks if user is authenticated.
 * Redirects to /login if not.
 */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthFacadeService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

/**
 * Guard that checks if user is admin.
 * Redirects to /login if not authenticated or not admin.
 */
export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthFacadeService);
  const router = inject(Router);

  if (!auth.isAuthenticated()) {
    return router.createUrlTree(['/login']);
  }

  if (auth.isAdmin()) {
    return true;
  }

  return router.createUrlTree(['/']); // or /unauthorized
};
