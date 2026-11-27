import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../admin-users/admin-users-api.service';

export interface AdminReview {
  id: number;
  username: string;
  gameTitle: string;
  content: string;
  createdAt: string;
  gameId: number | null;
  userId: number | null;
  rating: number | null;   // ocjena autora za igru (1–5), može biti null
  isReported: boolean;     // korisnik je prijavio recenziju
}

export interface ReviewFilter {
  game?: string | null;
  user?: string | null;
  minRating?: number | null;
  createdFrom?: string | null;
  createdTo?: string | null;
  reportedOnly?: boolean | null;
  page: number;
  pageSize: number;
}

@Injectable({ providedIn: 'root' })
export class AdminReviewsApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Admin/comments`;

  /** GET .../paged – filteri i paging rade na backendu */
  getPaged(filter: ReviewFilter): Observable<PagedResult<AdminReview>> {
    let params = new HttpParams();
    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '' && value !== false) params = params.set(key, String(value));
    });
    return this.http.get<PagedResult<AdminReview>>(`${this.baseUrl}/paged`, { params });
  }

  /** PUT .../dismiss-report – recenzija je u redu, ukloni prijavu */
  dismissReport(id: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/dismiss-report`, null);
  }

  /** DELETE – backend vraća običan tekst */
  delete(id: number): Observable<string> {
    return this.http.delete(`${this.baseUrl}/${id}`, { responseType: 'text' });
  }
}
