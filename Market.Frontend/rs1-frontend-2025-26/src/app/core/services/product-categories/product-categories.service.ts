import {inject, Injectable} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  ListProductCategoriesRequest,
  ListProductCategoriesResponse,
  ProductCategoryListItem, UpsertProductCategoryCommand
} from './product-categories.model';
import { buildHttpParams } from '../../models/buildHttpParams';

@Injectable({
  providedIn: 'root',
})
export class ProductCategoriesService {
  private readonly baseUrl = `${environment.apiUrl}/ProductCategories`;
  private http = inject(HttpClient);

  /**
   * Lista kategorija.
   * Trenutno bez paginga u komponenti, ali backend može i dalje podržavati paging/filtere.
   */
  list(request?: ListProductCategoriesRequest): Observable<ListProductCategoriesResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListProductCategoriesResponse>(this.baseUrl, {
      params,
    });
  }

  getById(id: number): Observable<ProductCategoryListItem> {
    return this.http.get<ProductCategoryListItem>(`${this.baseUrl}/${id}`);
  }

  create(payload: UpsertProductCategoryCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpsertProductCategoryCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
