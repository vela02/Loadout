import {inject, Injectable} from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";
import {
  ListProductsRequest,
  ListProductsResponse,
  CreateProductCommand,
  UpdateProductCommand
} from "./products.models";
import {buildHttpParams} from '../../core/models/buildHttpParams';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {

  private readonly baseUrl = `${environment.apiUrl}/Products`;
  private http = inject(HttpClient);

  list(request: ListProductsRequest): Observable<ListProductsResponse> {
    return this.http.get<ListProductsResponse>(
      this.baseUrl,
      { params: buildHttpParams(request) }
    );
  }

  getById(id: number) {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateProductCommand) {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateProductCommand) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
