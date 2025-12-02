import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthStateService } from './auth-state.service';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private authState = inject(AuthStateService);
  private authService = inject(AuthService);

  private refreshInProgress = false;
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // 1) ne diramo Auth endpoint-e (login/refresh/logout)
    if (this.isAuthEndpoint(req.url)) {
      return next.handle(req);
    }

    // 2) dodaj Authorization header ako ima token
    const accessToken = this.authState.accessToken;
    let authReq = req;

    if (accessToken) {
      authReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
    }

    // 3) handle 401 → refresh → retry
    return next.handle(authReq).pipe(
      catchError((err) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => err);
      })
    );
  }

  private isAuthEndpoint(url: string): boolean {
    // prilagodi po potrebi (npr. /api/Auth)
    return url.includes('/Auth/');
  }

  private handle401Error(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const refreshToken = this.authState.refreshToken;
    if (!refreshToken) {
      // nema refresh tokena → logout scenarij
      this.authState.clear();
      return throwError(() => new Error('No refresh token'));
    }

    // ako je refresh već u toku → sačekaj rezultat
    if (this.refreshInProgress) {
      return this.refreshTokenSubject.pipe(
        filter((token) => token !== null),
        take(1),
        switchMap((token) => {
          const cloned = token
            ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
            : req;
          return next.handle(cloned);
        })
      );
    }

    // prvi koji je dobio 401 pokreće refresh
    this.refreshInProgress = true;
    this.refreshTokenSubject.next(null);

    return this.authService
      .refresh({
        refreshToken: refreshToken,
        fingerprint: undefined,
      })
      .pipe(
        switchMap((res) => {
          this.refreshInProgress = false;
          this.authState.setRefresh(res);
          this.refreshTokenSubject.next(res.accessToken);

          const clonedReq = req.clone({
            setHeaders: {
              Authorization: `Bearer ${res.accessToken}`,
            },
          });

          return next.handle(clonedReq);
        }),
        catchError((error) => {
          this.refreshInProgress = false;
          this.authState.clear();
          this.refreshTokenSubject.next(null);
          return throwError(() => error);
        })
      );
  }
}
