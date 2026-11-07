import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface JobStatus {
  name: string;
  description: string;
  intervalSeconds: number;
  isRunning: boolean;
  lastRunAt: string | null;
  nextRunAt: string | null;
  lastSuccess: boolean | null;
  lastResult: string | null;
  totalRuns: number;
  failedRuns: number;
}

export interface JobRun {
  jobName: string;
  startedAt: string;
  durationMs: number;
  success: boolean;
  result: string;
  trigger: 'Scheduler' | 'Manual';
}

export interface JobsSnapshot {
  jobs: JobStatus[];
  history: JobRun[];
}

@Injectable({ providedIn: 'root' })
export class AdminJobsApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/Admin/jobs`;

  getStatus(): Observable<JobsSnapshot> {
    return this.http.get<JobsSnapshot>(this.baseUrl);
  }

  /** Ručno pokretanje ("Pokreni sada") – vraća novo stanje */
  run(name: string): Observable<JobsSnapshot> {
    return this.http.post<JobsSnapshot>(`${this.baseUrl}/${name}/run`, null);
  }
}
