// src/app/core/services/auth/auth-state.service.ts
import { Injectable } from '@angular/core';
import { LoginResponse, RefreshTokenResponse } from './auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private readonly accessTokenKey = 'accessToken';
  private readonly refreshTokenKey = 'refreshToken';
  private readonly accessTokenExpiresAtKey = 'accessTokenExpiresAtUtc';
  private readonly refreshTokenExpiresAtKey = 'refreshTokenExpiresAtUtc';

  setLogin(response: LoginResponse): void {
    localStorage.setItem(this.accessTokenKey, response.accessToken);
    localStorage.setItem(this.refreshTokenKey, response.refreshToken);
    localStorage.setItem(this.accessTokenExpiresAtKey, response.expiresAtUtc);
  }

  setRefresh(response: RefreshTokenResponse): void {
    localStorage.setItem(this.accessTokenKey, response.accessToken);
    localStorage.setItem(this.refreshTokenKey, response.refreshToken);
    localStorage.setItem(this.accessTokenExpiresAtKey, response.accessTokenExpiresAtUtc);
    localStorage.setItem(this.refreshTokenExpiresAtKey, response.refreshTokenExpiresAtUtc);
  }

  clear(): void {
    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    localStorage.removeItem(this.accessTokenExpiresAtKey);
    localStorage.removeItem(this.refreshTokenExpiresAtKey);
  }

  get accessToken(): string | null {
    return localStorage.getItem(this.accessTokenKey);
  }

  get refreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.accessToken;
  }
}
