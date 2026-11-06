import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CheckoutResult } from '../games/games-api.models';

export interface PaymentIntentResponse {
  clientSecret: string;      // tajna za potvrdu plaćanja u Stripe.js (vrijedi samo za ovu transakciju)
  paymentIntentId: string;   // pi_...
  amount: number;
  currency: string;
}

/** Stripe naplata. Korisnik se na backendu čita iz JWT tokena. */
@Injectable({ providedIn: 'root' })
export class PaymentsApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Payments`;

  /** Publishable key (pk_test_...) za učitavanje Stripe.js */
  getConfig(): Observable<{ publishableKey: string }> {
    return this.http.get<{ publishableKey: string }>(`${this.baseUrl}/config`);
  }

  /** Backend izračuna iznos korpe i kod Stripe-a otvori PaymentIntent */
  createIntent(): Observable<PaymentIntentResponse> {
    return this.http.post<PaymentIntentResponse>(`${this.baseUrl}/create-intent`, null);
  }

  /** Backend provjeri kod Stripe-a da je plaćanje uspjelo i kreira narudžbe */
  confirm(paymentIntentId: string): Observable<CheckoutResult> {
    const params = new HttpParams().set('paymentIntentId', paymentIntentId);
    return this.http.post<CheckoutResult>(`${this.baseUrl}/confirm`, null, { params });
  }
}
