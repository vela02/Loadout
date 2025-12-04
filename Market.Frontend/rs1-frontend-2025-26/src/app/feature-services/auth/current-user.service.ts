// src/app/core/auth/current-user.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import {jwtDecode} from 'jwt-decode';
import { AuthStorageService } from './auth-storage.service';
import {CurrentUser} from '../../core/models/currentUser';
import {MyJwtPayload} from '../../core/models/myJwtPayload';

@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  private readonly currentUserSubject = new BehaviorSubject<CurrentUser | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private authState: AuthStorageService) {
    this.recalculateFromToken();
  }

  /** Ručno pozoveš nakon login/refresh-a */
  recalculateFromToken(): void {
    const token = this.authState.getAccessToken();
    if (!token) {
      this.currentUserSubject.next(null);
      return;
    }

    try {
      const payload = jwtDecode<MyJwtPayload>(token);
      const user: CurrentUser = {
        userId: Number(payload.sub),
        email: payload.email,
        isAdmin: payload.is_admin === 'true',
        isManager: payload.is_manager === 'true',
        isEmployee: payload.is_employee === 'true',
        tokenVersion: Number(payload.ver),
      };

      this.currentUserSubject.next(user);
    } catch {
      this.currentUserSubject.next(null);
    }
  }

  get snapshot(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return !!this.snapshot;
  }

  get isAdmin(): boolean {
    return !!this.snapshot?.isAdmin;
  }
}
