// src/app/core/services/auth/auth-facade.service.ts
import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { tap, map, catchError } from 'rxjs/operators';
import {
  LoginRequest,
  LoginResponse,
  LogoutRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
} from './auth.model';
import { AuthStateService } from './auth-state.service';
import {AuthApiService} from './auth-api.service';
import {CurrentUserService} from './current-user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthFacadeService {
  private currentUser = inject(CurrentUserService);
  private api = inject(AuthApiService);        // HTTP prema /api/Auth
  private state = inject(AuthStateService); // localStorage state

  // === GETTERI ZA TOKENE ===
  get accessToken(): string | null {
    return this.state.accessToken;
  }

  get refreshToken(): string | null {
    return this.state.refreshToken;
  }

  clearState(): void {
    this.state.clear();
  }

  // === LOGIN ===
  login(payload: LoginRequest): Observable<void> {
    return this.api.login(payload).pipe(
      tap((res: LoginResponse) => {
        this.state.setLogin(res);
        this.currentUser.recalculateFromToken();
      }),
      map(() => void 0)
    );
  }

  // === LOGOUT ===
  logout(): Observable<void> {
    const refreshToken = this.refreshToken;

    if (!refreshToken) {
      this.clearState();
      return of(void 0);
    }

    const payload: LogoutRequest = { refreshToken };

    return this.api.logout(payload).pipe(
      catchError(() => of(void 0)),
      tap(() => this.clearState())
    );
  }

  // === REFRESH TOKEN ===
  refresh(payload: RefreshTokenRequest): Observable<RefreshTokenResponse> {
    return this.api.refresh(payload).pipe(
      tap((res: RefreshTokenResponse) => {
        this.state.setRefresh(res);
        this.currentUser.recalculateFromToken();
      })
    );
  }
}
