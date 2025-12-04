import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { AuthApiService } from '../../api-services/auth/auth-api.service';
import { AuthStorageService } from './auth-storage.service';
import {
  LoginCommand,
  LoginCommandDto,
  LogoutCommand,
  RefreshTokenCommand,
  RefreshTokenCommandDto
} from '../../api-services/auth/auth-api.model';
import { CurrentUser } from '../../core/models/currentUser';
import { MyJwtPayload } from '../../core/models/myJwtPayload';

/**
 * Main authentication service.
 * Handles login, logout, token refresh, and current user state.
 *
 * Use this service in:
 * - Components (login, logout, navbar)
 * - Guards (auth guard, role guard)
 * - Interceptors (auth interceptor)
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private api = inject(AuthApiService);
  private storage = inject(AuthStorageService);
  private router = inject(Router);

  // Reactive state - current user as signal
  private _currentUser = signal<CurrentUser | null>(null);

  // Public readonly signals
  currentUser = this._currentUser.asReadonly();
  isAuthenticated = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.isAdmin ?? false);
  isManager = computed(() => this._currentUser()?.isManager ?? false);
  isEmployee = computed(() => this._currentUser()?.isEmployee ?? false);

  constructor() {
    // Initialize current user from token on service creation
    this.initializeFromToken();
  }

  // === PUBLIC API ===

  /**
   * Login user with email and password.
   */
  login(payload: LoginCommand): Observable<void> {
    return this.api.login(payload).pipe(
        tap((response: LoginCommandDto) => {
          this.storage.saveLogin(response);
          this.decodeAndSetUser(response.accessToken);
        }),
        map(() => void 0)
    );
  }

  /**
   * Logout user - invalidate refresh token on server and clear local state.
   */
  logout(): Observable<void> {
    const refreshToken = this.storage.getRefreshToken();

    // Clear local state first (optimistic)
    this.clearUserState();

    // If no refresh token, just return
    if (!refreshToken) {
      return of(void 0);
    }

    // Try to invalidate on server (but don't block on error)
    const payload: LogoutCommand = { refreshToken };
    return this.api.logout(payload).pipe(
        catchError(() => of(void 0))
    );
  }

  /**
   * Refresh access token using refresh token.
   * Used by auth interceptor.
   */
  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.api.refresh(payload).pipe(
        tap((response: RefreshTokenCommandDto) => {
          this.storage.saveRefresh(response);
          this.decodeAndSetUser(response.accessToken);
        })
    );
  }

  /**
   * Navigate to login page and clear auth state.
   */
  redirectToLogin(): void {
    this.clearUserState();
    this.router.navigate(['/login']);
  }

  // === GETTERS FOR INTERCEPTOR ===

  /**
   * Get access token for Authorization header.
   */
  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  /**
   * Get refresh token for refresh request.
   */
  getRefreshToken(): string | null {
    return this.storage.getRefreshToken();
  }

  // === PRIVATE HELPERS ===

  /**
   * Initialize current user from stored token on service creation.
   */
  private initializeFromToken(): void {
    const token = this.storage.getAccessToken();
    if (token) {
      this.decodeAndSetUser(token);
    }
  }

  /**
   * Decode JWT token and set current user.
   */
  private decodeAndSetUser(token: string): void {
    try {
      const payload = jwtDecode<MyJwtPayload>(token);

      const user: CurrentUser = {
        userId: Number(payload.sub),
        email: payload.email,
        isAdmin: payload.is_admin === 'true',
        isManager: payload.is_manager === 'true',
        isEmployee: payload.is_employee === 'true',
        tokenVersion: Number(payload.ver)
      };

      this._currentUser.set(user);
    } catch (error) {
      console.error('Failed to decode JWT token:', error);
      this._currentUser.set(null);
    }
  }

  /**
   * Clear user state and remove tokens from storage.
   */
  private clearUserState(): void {
    this._currentUser.set(null);
    this.storage.clear();
  }
}
