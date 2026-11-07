import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../admin-users/admin-users-api.service';

export type AnnouncementType = 'Sale' | 'Info' | 'Maintenance' | 'Update';

export interface AdminAnnouncement {
  id: number;
  title: string;
  message: string;
  type: AnnouncementType;
  isActive: boolean;
  createdAt: string;
  publishAt: string | null;   // zakazana objava
  expiresAt: string | null;   // automatsko skrivanje (radi pozadinski proces)
}

export interface AnnouncementFilter {
  title?: string | null;
  type?: string | null;
  isActive?: boolean | null;
  createdFrom?: string | null;
  createdTo?: string | null;
  page: number;
  pageSize: number;
}

export interface AnnouncementUpsert {
  title: string;
  message: string;
  type: AnnouncementType;
  isActive: boolean;
  publishAt: string | null;
  expiresAt: string | null;
}

@Injectable({ providedIn: 'root' })
export class AdminNewsApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Admin/announcements`;

  /** GET – filteri i paging rade na backendu */
  getPaged(filter: AnnouncementFilter): Observable<PagedResult<AdminAnnouncement>> {
    let params = new HttpParams();
    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') params = params.set(key, String(value));
    });
    return this.http.get<PagedResult<AdminAnnouncement>>(this.baseUrl, { params });
  }

  /** POST – backend vraća običan tekst */
  create(dto: AnnouncementUpsert): Observable<string> {
    return this.http.post(this.baseUrl, dto, { responseType: 'text' });
  }

  update(id: number, dto: AnnouncementUpsert): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  /** PUT .../toggle – uključi/isključi prikaz u javnom feedu */
  toggle(id: number): Observable<string> {
    return this.http.put(`${this.baseUrl}/${id}/toggle`, null, { responseType: 'text' });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
