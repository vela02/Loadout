import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreatePromotionCommand,
  GetPromotionOptionsQueryDto,
  ListPromotionsQueryDto
} from './promotions-api.models';

@Injectable({
  providedIn: 'root'
})
export class PromotionsApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/admin/promotions`;
  private http = inject(HttpClient);

  /**
   * GET /api/admin/promotions
   * List all promotions (newest start date first).
   */
  list(): Observable<ListPromotionsQueryDto[]> {
    return this.http.get<ListPromotionsQueryDto[]>(this.baseUrl);
  }

  /**
   * GET /api/admin/promotions/options
   * Categories, tags, genres, publishers, developers and games for the promotion form.
   */
  getOptions(): Observable<GetPromotionOptionsQueryDto> {
    return this.http.get<GetPromotionOptionsQueryDto>(`${this.baseUrl}/options`);
  }

  /**
   * POST /api/admin/promotions
   * Create a promotion; discounts are created for every matching game.
   * @returns ID of the newly created promotion
   */
  create(payload: CreatePromotionCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  /**
   * DELETE /api/admin/promotions/{id}
   * Delete a promotion together with its discounts.
   */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
