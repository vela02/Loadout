import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Announcement, DashboardStats, GameDetails, GameListItem } from './games-api.models';

@Injectable({ providedIn: 'root' })
export class GamesApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  /** GET /api/Games?searchTerm=...&genre=... -> lista igara (opcionalno filtrirana po nazivu i žanru) */
  list(searchTerm?: string, genre?: string | null): Observable<GameListItem[]> {
    let params = new HttpParams();
    if (searchTerm && searchTerm.trim()) {
      params = params.set('searchTerm', searchTerm.trim());
    }
    if (genre) {
      params = params.set('genre', genre); // backend filter po žanru (tačno poklapanje)
    }
    return this.http.get<GameListItem[]>(`${this.baseUrl}/api/Games`, { params });
  }

  /** GET /api/Games/{id} -> detalji jedne igre */
  getById(id: number): Observable<GameDetails> {
    return this.http.get<GameDetails>(`${this.baseUrl}/api/Games/${id}`);
  }

  /** POST /api/Order/add-to-cart -> dodaje igru u korpu (za pre-order igre backend pravi rezervaciju) */
  addToCart(userId: number, gameId: number): Observable<string> {
    const params = new HttpParams()
      .set('userId', userId)
      .set('gameId', gameId)
      .set('quantity', 1);
    // backend vraća običan tekst, ne JSON -> responseType: 'text'
    return this.http.post(`${this.baseUrl}/api/Order/add-to-cart`, null, { params, responseType: 'text' });
  }

  /** GET /api/Admin/stats -> statistika za admin dashboard (samo Administrator) */
  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.baseUrl}/api/Admin/stats`);
  }

  /** GET /api/notifications -> globalne vijesti i obavijesti (samo aktivne) */
  getAnnouncements(): Observable<Announcement[]> {
    return this.http.get<Announcement[]>(`${this.baseUrl}/api/notifications`);
  }
}
