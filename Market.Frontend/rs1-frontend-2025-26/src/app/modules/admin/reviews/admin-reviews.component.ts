import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { AdminReview, AdminReviewsApiService, ReviewFilter } from '../../../api-services/admin-reviews/admin-reviews-api.service';
import { PagedResult } from '../../../api-services/admin-users/admin-users-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';

/**
 * ADMIN – MODERACIJA RECENZIJA I KOMENTARA
 *  - tabela: igra, korisnik, ocjena, tekst, datum, prijava
 *  - filter sa 6 parametara (igra, korisnik, min. ocjena, od/do, samo prijavljene)
 *  - backend paging, prijavljene recenzije su uvijek na vrhu
 *  - brisanje uz potvrdu (nestaje i sa stranice igre) / odbacivanje prijave
 */
@Component({
  selector: 'app-admin-reviews',
  standalone: false,
  templateUrl: './admin-reviews.component.html',
  styleUrl: './admin-reviews.component.scss',
})
export class AdminReviewsComponent implements OnInit, OnDestroy {
  private api = inject(AdminReviewsApiService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private dialogHelper = inject(DialogHelperService);

  readonly pageSizes = [5, 10, 20];
  readonly ratings = [1, 2, 3, 4, 5];
  readonly stars = [1, 2, 3, 4, 5];

  result: PagedResult<AdminReview> | null = null;
  loading = false;
  page = 1;
  pageSize = 10;
  expandedId: number | null = null;   // klik na tekst -> prikaži cijelu recenziju

  // ---------- FILTERI ----------
  filterForm = this.fb.group({
    game: [''],
    user: [''],
    minRating: [null as number | null],
    createdFrom: [''],
    createdTo: [''],
    reportedOnly: [false],
  });

  private sub?: Subscription;

  ngOnInit(): void {
    this.load();
    this.sub = this.filterForm.valueChanges.pipe(debounceTime(350)).subscribe(() => {
      this.page = 1;
      this.load();
    });
  }

  load(): void {
    const f = this.filterForm.getRawValue();
    // Datum "od" poslije "do" -> ne šaljemo upit (backend bi vratio 400)
    if (this.dateRangeInvalid) return;
    const filter: ReviewFilter = { ...f, page: this.page, pageSize: this.pageSize };
    this.loading = true;
    this.api.getPaged(filter).subscribe({
      next: (r) => {
        this.result = r;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  get dateRangeInvalid(): boolean {
    const { createdFrom, createdTo } = this.filterForm.getRawValue();
    return !!createdFrom && !!createdTo && createdFrom > createdTo;
  }

  get activeFilterCount(): number {
    const f = this.filterForm.getRawValue();
    return [f.game, f.user, f.minRating, f.createdFrom, f.createdTo, f.reportedOnly || null]
      .filter((v) => v !== null && v !== '').length;
  }

  resetFilters(): void {
    this.filterForm.reset({ game: '', user: '', minRating: null, createdFrom: '', createdTo: '', reportedOnly: false });
  }

  // ---------- PAGING ----------
  goToPage(p: number): void {
    if (!this.result || p < 1 || p > this.result.totalPages) return;
    this.page = p;
    this.load();
  }

  changePageSize(size: number): void {
    this.pageSize = Number(size);
    this.page = 1;
    this.load();
  }

  get pages(): number[] {
    const total = this.result?.totalPages ?? 0;
    const start = Math.max(1, this.page - 2);
    const end = Math.min(total, start + 4);
    return Array.from({ length: Math.max(0, end - start + 1) }, (_, i) => start + i);
  }

  // ---------- AKCIJE ----------
  toggleExpand(r: AdminReview): void {
    this.expandedId = this.expandedId === r.id ? null : r.id;
  }

  isLong(text: string): boolean {
    return text.length > 120;
  }

  dismiss(r: AdminReview): void {
    this.api.dismissReport(r.id).subscribe(() => {
      r.isReported = false;   // odmah osvježi prikaz
      this.toaster.success(this.translate.instant('GAMES.REVIEWS_ADMIN.DISMISSED'));
      if (this.filterForm.controls.reportedOnly.value) this.load(); // više ne spada u filter
    });
  }

  remove(r: AdminReview): void {
    this.dialogHelper
      .confirm('GAMES.REVIEWS_ADMIN.DELETE_TITLE', 'GAMES.REVIEWS_ADMIN.DELETE_MESSAGE', { user: r.username, game: r.gameTitle })
      .subscribe((res) => {
        if (res?.button !== DialogButton.YES) return;
        this.api.delete(r.id).subscribe({
          next: () => {
            this.toaster.success(this.translate.instant('GAMES.REVIEWS_ADMIN.DELETED'));
            // obrisali smo zadnju stavku na stranici -> vrati se stranicu nazad
            if (this.result && this.result.items.length === 1 && this.page > 1) this.page--;
            this.load();
          },
          error: () => this.toaster.error(this.translate.instant('GAMES.REVIEWS_ADMIN.DELETE_FAILED')),
        });
      });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
