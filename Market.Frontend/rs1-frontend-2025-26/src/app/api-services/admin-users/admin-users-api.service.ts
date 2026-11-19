import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { OrderHistory } from '../games/games-api.models';

export interface AdminUser {
  id: number;
  username: string;
  email: string;
  roleId: number;
  roleName: string;
  isEnabled: boolean;
  createdAt: string;
  lastLogin: string | null;
}

export interface Role {
  id: number;
  name: string;
}

export interface UserFilter {
  search?: string | null;
  roleId?: number | null;
  isEnabled?: boolean | null;
  createdFrom?: string | null;   // "2026-01-31"
  createdTo?: string | null;
  sortBy: string;
  sortDir: 'asc' | 'desc';
  page: number;
  pageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  pageSize: number;
  currentPage: number;
  totalItems: number;
  totalPages: number;
}

/** Historija kupovina jednog korisnika (admin) – podaci, statistika i narudžbe u periodu */
export interface UserPurchaseHistory {
  userId: number;
  username: string;
  email: string;
  roleName: string;
  registeredAt: string;
  from: string | null;
  to: string | null;
  ordersCount: number;
  gamesCount: number;
  totalSpent: number;      // bez otkazanih narudžbi
  orders: OrderHistory[];
}

export interface UserUpsert {
  username: string;
  email: string;
  roleId: number;
  isEnabled: boolean;
  password?: string | null;
}

@Injectable({ providedIn: 'root' })
export class AdminUsersApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/admin/users`;

  /** GET /api/admin/users/paged -> filteri, sortiranje i paging rade na BACKENDU */
  getPaged(filter: UserFilter): Observable<PagedResult<AdminUser>> {
    let params = new HttpParams();
    // u query string ide samo ono što je postavljeno
    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        params = params.set(key, String(value));
      }
    });
    return this.http.get<PagedResult<AdminUser>>(`${this.baseUrl}/paged`, { params });
  }

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.baseUrl}/roles`);
  }

  create(dto: UserUpsert): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl, dto);
  }

  update(id: number, dto: UserUpsert): Observable<boolean> {
    return this.http.put<boolean>(`${this.baseUrl}/${id}`, dto);
  }

  /** GET /api/admin/users/{id}/purchases?from=&to= */
  getPurchases(id: number, from?: string | null, to?: string | null): Observable<UserPurchaseHistory> {
    let params = new HttpParams();
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);
    return this.http.get<UserPurchaseHistory>(`${this.baseUrl}/${id}/purchases`, { params });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
