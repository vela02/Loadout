// src/app/core/services/auth/auth.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthStateService } from './auth-state.service';
import { AuthService } from './auth.service';

// Globalni state za refresh (van funkcije da se dijeli između poziva)
let refreshInProgress = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authState = inject(AuthStateService);
  const authService = inject(AuthService);

  // 1) Ne diramo Auth endpoint-e (login/refresh/logout)
  if (isAuthEndpoint(req.url)) {
    return next(req);
  }

  // 2) Dodaj Authorization header ako ima token
  const accessToken = authState.accessToken;
  let authReq = req;

  if (accessToken) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
  }

  // 3) Handle 401 → refresh → retry
  return next(authReq).pipe(
    catchError((err) => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        return handle401Error(authReq, next, authState, authService);
      }
      return throwError(() => err);
    })
  );
};

// Helper funkcije
function isAuthEndpoint(url: string): boolean {
  // Prilagodi po potrebi (npr. /api/Auth)
  return url.includes('/Auth/');
}

function handle401Error(
  req: any,
  next: any,
  authState: AuthStateService,
  authService: AuthService
): Observable<any> {
  const refreshToken = authState.refreshToken;

  if (!refreshToken) {
    // Nema refresh tokena → logout scenarij
    authState.clear();
    return throwError(() => new Error('No refresh token'));
  }

  // Ako je refresh već u toku → sačekaj rezultat
  if (refreshInProgress) {
    return refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => {
        const cloned = token
          ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
          : req;
        return next(cloned);
      })
    );
  }

  // Prvi koji je dobio 401 pokreće refresh
  refreshInProgress = true;
  refreshTokenSubject.next(null);

  return authService
    .refresh({
      refreshToken: refreshToken,
      fingerprint: undefined,
    })
    .pipe(
      switchMap((res) => {
        refreshInProgress = false;
        authState.setRefresh(res);
        refreshTokenSubject.next(res.accessToken);

        const clonedReq = req.clone({
          setHeaders: {
            Authorization: `Bearer ${res.accessToken}`,
          },
        });

        return next(clonedReq);
      }),
      catchError((error) => {
        refreshInProgress = false;
        authState.clear();
        refreshTokenSubject.next(null);
        return throwError(() => error);
      })
    );
}
