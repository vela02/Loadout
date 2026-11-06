import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Announcement, CartItem, CheckoutResult, DashboardStats, GameDetails, GameListItem, OrderHistory,
} from './games-api.models';

/** Napredni filteri kataloga */
export interface CatalogFilters {
  minPrice: number | null;
  maxPrice: number | null;
  minRating: number | null;   // 1–5
  type: number | null;        // 0 = Igra, 1 = DLC, 2 = Dodatak, 3 = Item
  releasedFrom: string | null; // "2020-01-01" (iz <input type="date">)
  releasedTo: string | null;
}

@Injectable({ providedIn: 'root' })
export class GamesApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  /** GET /api/Games?searchTerm=...&genre=... -> lista igara (opcionalno filtrirana po nazivu i žanru) */
  list(searchTerm?: string, genre?: string | null, filters?: CatalogFilters): Observable<GameListItem[]> {
    let params = new HttpParams();
    if (searchTerm && searchTerm.trim()) {
      params = params.set('searchTerm', searchTerm.trim());
    }
    if (genre) {
      params = params.set('genre', genre); // backend filter po žanru (tačno poklapanje)
    }
    // Napredni filteri -> isti nazivi kao [FromQuery] parametri u GamesController
    if (filters?.minPrice != null) params = params.set('minPrice', filters.minPrice);
    if (filters?.maxPrice != null) params = params.set('maxPrice', filters.maxPrice);
    if (filters?.minRating != null) params = params.set('minRating', filters.minRating);
    if (filters?.type != null) params = params.set('type', filters.type);
    if (filters?.releasedFrom) params = params.set('releasedFrom', filters.releasedFrom);
    if (filters?.releasedTo) params = params.set('releasedTo', filters.releasedTo);
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

  // ---------- KORPA ----------

  /** GET /api/Order/cart/{userId} -> sve stavke (i aktivne i "za kasnije") */
  getCart(userId: number): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.baseUrl}/api/Order/cart/${userId}`);
  }

  /** DELETE /api/Order/cart/{userId}/items/{cartItemId} */
  removeFromCart(userId: number, cartItemId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/api/Order/cart/${userId}/items/${cartItemId}`);
  }

  /** PUT .../save-for-later?saved=true|false -> premjesti u "za kasnije" ili nazad u korpu */
  setSavedForLater(userId: number, cartItemId: number, saved: boolean): Observable<void> {
    const params = new HttpParams().set('saved', saved);
    return this.http.put<void>(
      `${this.baseUrl}/api/Order/cart/${userId}/items/${cartItemId}/save-for-later`, null, { params });
  }

  /** POST /api/Order/save-for-later -> igra ide direktno u "sačuvano za kasnije" */
  saveForLater(userId: number, gameId: number): Observable<void> {
    const params = new HttpParams().set('userId', userId).set('gameId', gameId);
    return this.http.post<void>(`${this.baseUrl}/api/Order/save-for-later`, null, { params });
  }

  /** POST /api/Order/checkout/{userId} -> kreira narudžbu od aktivnih stavki */
  checkout(userId: number): Observable<CheckoutResult> {
    return this.http.post<CheckoutResult>(`${this.baseUrl}/api/Order/checkout/${userId}`, null);
  }

  /** GET /api/Order/history/{userId} -> narudžbe s licencnim ključevima */
  getOrderHistory(userId: number): Observable<OrderHistory[]> {
    return this.http.get<OrderHistory[]>(`${this.baseUrl}/api/Order/history/${userId}`);
  }

  /** POST /api/Order/cancel-preorder/{orderId} -> otkazuje pre-order (backend vraća tekst) */
  cancelPreOrder(orderId: number): Observable<string> {
    return this.http.post(`${this.baseUrl}/api/Order/cancel-preorder/${orderId}`, null, { responseType: 'text' });
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
