// src/app/core/services/auth/auth.interceptor.ts
import {
    HttpInterceptorFn,
    HttpErrorResponse,
    HttpRequest,
    HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthFacadeService } from './auth-facade.service';

// Globalni state za refresh (dijeli se između poziva)
let refreshInProgress = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const auth = inject(AuthFacadeService);

    // 1) Ne diramo Auth endpoint-e (login/refresh/logout)
    if (isAuthEndpoint(req.url)) {
        return next(req);
    }

    // 2) Dodaj Authorization header ako ima token
    const accessToken = auth.accessToken;
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
                return handle401Error(authReq, next, auth);
            }
            return throwError(() => err);
        })
    );
};

// Helper funkcije
function isAuthEndpoint(url: string): boolean {
    // prilagodi ako ti je base /api/Auth
    return url.includes('/Auth/');
}

function handle401Error(
    req: HttpRequest<unknown>,
    next: HttpHandlerFn,
    auth: AuthFacadeService
): Observable<any> {
    const refreshToken = auth.refreshToken;

    if (!refreshToken) {
        // Nema refresh tokena → logout scenarij
        auth.clearState();
        return throwError(() => new Error('No refresh token'));
    }

    // Ako je refresh već u toku → sačekaj rezultat
    if (refreshInProgress) {
        return refreshTokenSubject.pipe(
            filter((token) => token !== null),
            take(1),
            switchMap((token) => {
                const cloned = token
                    ? req.clone({
                        setHeaders: { Authorization: `Bearer ${token}` },
                    })
                    : req;
                return next(cloned);
            })
        );
    }

    // Prvi koji je dobio 401 pokreće refresh
    refreshInProgress = true;
    refreshTokenSubject.next(null);

    return auth
        .refresh({
            refreshToken: refreshToken,
            fingerprint: undefined,
        })
        .pipe(
            switchMap((res) => {
                refreshInProgress = false;

                // state je već updatovan u AuthFacadeService.refresh()
                const newAccessToken = res.accessToken;
                refreshTokenSubject.next(newAccessToken);

                const clonedReq = req.clone({
                    setHeaders: {
                        Authorization: `Bearer ${newAccessToken}`,
                    },
                });

                return next(clonedReq);
            }),
            catchError((error) => {
                refreshInProgress = false;
                auth.clearState();
                refreshTokenSubject.next(null);
                return throwError(() => error);
            })
        );
}
