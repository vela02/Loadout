import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ChangePasswordCommand, GetProfileDto } from './users-api.models';

@Injectable({
  providedIn: 'root'
})
export class UsersApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/users`;
  private http = inject(HttpClient);

  /**
   * GET /api/users/profile
   * Profile and account overview of the logged-in user.
   */
  getProfile(): Observable<GetProfileDto> {
    return this.http.get<GetProfileDto>(`${this.baseUrl}/profile`);
  }

  /**
   * POST /api/users/change-password
   * Backend returns a plain text message, so the response is read as text.
   */
  changePassword(payload: ChangePasswordCommand): Observable<void> {
    return this.http
      .post(`${this.baseUrl}/change-password`, payload, { responseType: 'text' })
      .pipe(map(() => void 0));
  }
}
