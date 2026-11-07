import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { AdminJobsApiService, JobsSnapshot, JobStatus } from '../../../api-services/admin-jobs/admin-jobs-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

/**
 * UVID U STANJE POZADINSKIH PROCESA:
 * status svakog zadatka (radi / uspjeh / greška), zadnje i sljedeće pokretanje,
 * broj izvršavanja, historija i ručno pokretanje. Osvježava se automatski svakih 5 s.
 */
@Component({
  selector: 'app-admin-jobs',
  standalone: false,
  templateUrl: './admin-jobs.component.html',
  styleUrl: './admin-jobs.component.scss',
})
export class AdminJobsComponent implements OnInit, OnDestroy {
  private api = inject(AdminJobsApiService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  data: JobsSnapshot | null = null;
  running: string | null = null;
  private sub?: Subscription;

  ngOnInit(): void {
    // timer(0, 5000): odmah, pa svakih 5 s -> "živi" prikaz stanja
    this.sub = timer(0, 5000)
      .pipe(switchMap(() => this.api.getStatus()))
      .subscribe((d) => (this.data = d));
  }

  run(job: JobStatus): void {
    this.running = job.name;
    this.api.run(job.name).subscribe({
      next: (d) => {
        this.data = d;
        this.running = null;
        const last = d.jobs.find((j) => j.name === job.name);
        this.toaster.success(`${this.jobTitle(job.name)}: ${last?.lastResult ?? ''}`);
      },
      error: () => {
        this.running = null;
        this.toaster.error(this.translate.instant('GAMES.JOBS.RUN_FAILED'));
      },
    });
  }

  jobTitle(name: string): string {
    return this.translate.instant(`GAMES.JOBS.NAMES.${name}`);
  }

  jobIcon(name: string): string {
    return name === 'ReleasePreOrders' ? 'rocket_launch' : 'event_busy';
  }

  /** Koliko je ostalo do sljedećeg pokretanja, npr. "za 42 s" */
  secondsUntil(iso: string | null): number {
    if (!iso) return 0;
    return Math.max(0, Math.round((new Date(iso).getTime() - Date.now()) / 1000));
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
