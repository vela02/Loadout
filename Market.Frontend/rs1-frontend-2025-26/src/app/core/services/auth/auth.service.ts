import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {LoginRequest, LoginResponse, LogoutRequest, RefreshTokenRequest, RefreshTokenResponse} from './auth.model';


@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  private readonly baseUrl = `${environment.apiUrl}/Auth`;

  /**
   * POST /Auth/login
   * Body: LoginRequest (email, password, fingerprint?)
   * Returns: LoginResponse (access + refresh token)
   */
  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, payload);
  }

  /**
   * POST /Auth/refresh
   * Body: RefreshTokenRequest (refreshToken, fingerprint?)
   * Returns: RefreshTokenResponse (new access + refresh, with expiries)
   */
  refresh(payload: RefreshTokenRequest): Observable<RefreshTokenResponse> {
    return this.http.post<RefreshTokenResponse>(
      `${this.baseUrl}/refresh`,
      payload
    );
  }

  /**
   * POST /Auth/logout
   * Body: LogoutRequest (refreshToken)
   * Returns: void
   */
  logout(payload: LogoutRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, payload);
  }
}
