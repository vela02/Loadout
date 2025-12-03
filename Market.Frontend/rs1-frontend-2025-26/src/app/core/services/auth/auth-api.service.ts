import { HttpClient } from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {LoginRequest, LoginResponse, LogoutRequest, RefreshTokenRequest, RefreshTokenResponse} from './auth.model';
import {Observable} from 'rxjs';
import {environment} from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Auth`;

  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, payload);
  }

  refresh(payload: RefreshTokenRequest): Observable<RefreshTokenResponse> {
    return this.http.post<RefreshTokenResponse>(`${this.baseUrl}/refresh`, payload);
  }

  logout(payload: LogoutRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/logout`, payload);
  }
}
