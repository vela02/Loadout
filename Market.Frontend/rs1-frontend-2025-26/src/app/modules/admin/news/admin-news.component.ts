import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import {
  AdminAnnouncement, AdminNewsApiService, AnnouncementFilter, AnnouncementType,
} from '../../../api-services/admin-news/admin-news-api.service';
import { PagedResult } from '../../../api-services/admin-users/admin-users-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';

/**
 * ADMIN – VIJESTI I OBAVJEŠTENJA (CRUD)
 *  - filter sa 5 parametara (naslov, tip, status, objavljeno od/do)
 *  - backend paging
 *  - dodavanje / izmjena u modalu s FE validacijom (BE validacija: CreateNotificationDto)
 *  - aktiviranje / deaktiviranje (samo aktivne se vide u javnom feedu)
 *  - brisanje uz potvrdu
 */
@Component({
  selector: 'app-admin-news',
  standalone: false,
  templateUrl: './admin-news.component.html',
  styleUrl: './admin-news.component.scss',
})
export class AdminNewsComponent implements OnInit, OnDestroy {
  private api = inject(AdminNewsApiService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private dialogHelper = inject(DialogHelperService);

  readonly types: AnnouncementType[] = ['Sale', 'Info', 'Maintenance', 'Update'];
  readonly pageSizes = [5, 10, 20];

  result: PagedResult<AdminAnnouncement> | null = null;
  loading = false;
  page = 1;
  pageSize = 10;

  // ---------- FILTERI (5 parametara) ----------
  filterForm = this.fb.group({
    title: [''],
    type: [null as string | null],
    isActive: [null as boolean | null],
    createdFrom: [''],
    createdTo: [''],
  });

  // ---------- FORMA ZA DODAVANJE / IZMJENU ----------
  showForm = false;
  readonly now = new Date();
  editingId: number | null = null;
  saving = false;
  serverError = '';

  newsForm = this.fb.nonNullable.group(
    {
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(150)]],
      type: ['Info' as AnnouncementType, Validators.required],
      // HTML iz editora: dužinu provjeravamo na ČISTOM tekstu (bez tagova), a ukupnu veličinu HTML-a do 8000
      message: ['', [Validators.required, AdminNewsComponent.plainTextLength(10, 4000), Validators.maxLength(8000)]],
      isActive: [true],
      publishAt: [''],   // "2026-11-07T10:00" iz <input type="datetime-local">
      expiresAt: [''],
    },
    { validators: AdminNewsComponent.scheduleValidator },
  );

  /** Validator: dužina teksta BEZ HTML tagova (npr. "<p><b>Hi</b></p>" ima samo 2 znaka) */
  static plainTextLength(min: number, max: number) {
    return (control: AbstractControl): ValidationErrors | null => {
      const text = (control.value ?? '').replace(/<[^>]*>/g, '').replace(/&nbsp;/g, ' ').trim();
      if (!text) return null;                       // prazno hvata Validators.required
      return text.length < min || text.length > max ? { plainTextLength: { min, max, actual: text.length } } : null;
    };
  }

  /** Broj znakova bez formatiranja – za brojač ispod editora */
  get messageLength(): number {
    return (this.newsForm.controls.message.value ?? '').replace(/<[^>]*>/g, '').replace(/&nbsp;/g, ' ').trim().length;
  }

  /** Validator na nivou cijele forme: istek mora biti poslije objave */
  static scheduleValidator(group: AbstractControl): ValidationErrors | null {
    const publish = group.get('publishAt')?.value;
    const expires = group.get('expiresAt')?.value;
    return publish && expires && expires <= publish ? { scheduleOrder: true } : null;
  }

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
    const filter: AnnouncementFilter = { ...f, page: this.page, pageSize: this.pageSize };
    this.loading = true;
    this.api.getPaged(filter).subscribe({
      next: (r) => {
        this.result = r;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  get activeFilterCount(): number {
    const f = this.filterForm.getRawValue();
    return [f.title, f.type, f.isActive, f.createdFrom, f.createdTo].filter((v) => v !== null && v !== '').length;
  }

  resetFilters(): void {
    this.filterForm.reset({ title: '', type: null, isActive: null, createdFrom: '', createdTo: '' });
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

  // ---------- DODAVANJE / IZMJENA ----------
  openCreate(): void {
    this.editingId = null;
    this.serverError = '';
    this.newsForm.reset({ title: '', type: 'Info', message: '', isActive: true, publishAt: '', expiresAt: '' });
    this.showForm = true;
  }

  openEdit(n: AdminAnnouncement): void {
    this.editingId = n.id;
    this.serverError = '';
    this.newsForm.reset({
      title: n.title, type: n.type, message: n.message, isActive: n.isActive,
      publishAt: n.publishAt?.slice(0, 16) ?? '',   // "2026-11-07T10:00:00" -> "2026-11-07T10:00"
      expiresAt: n.expiresAt?.slice(0, 16) ?? '',
    });
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
  }

  save(): void {
    if (this.newsForm.invalid) {
      this.newsForm.markAllAsTouched();
      return;
    }
    const v = this.newsForm.getRawValue();
    const dto = { ...v, publishAt: v.publishAt || null, expiresAt: v.expiresAt || null }; // prazno -> null
    this.saving = true;
    this.serverError = '';

    // create() vraća tekst, update() ništa -> zajednički tip Observable<unknown> (odgovor nam ne treba)
    const request$: Observable<unknown> = this.editingId ? this.api.update(this.editingId, dto) : this.api.create(dto);
    request$.subscribe({
      next: () => {
        this.saving = false;
        this.showForm = false;
        this.toaster.success(this.translate.instant(this.editingId ? 'GAMES.NEWS_ADMIN.UPDATED' : 'GAMES.NEWS_ADMIN.CREATED'));
        this.load();
      },
      error: (err: HttpErrorResponse) => {
        this.saving = false;
        this.serverError = this.extractError(err);
      },
    });
  }

  /** Backend validacija vraća 400 s greškama po poljima – izvučemo prvu čitljivu poruku */
  private extractError(err: HttpErrorResponse): string {
    const e = err.error;
    if (typeof e === 'string') return e;
    if (e?.message) return e.message;
    if (e?.errors) {
      const first = Object.values(e.errors as Record<string, string[]>)[0];
      if (first?.length) return first[0];
    }
    return this.translate.instant('GAMES.USERS.SAVE_FAILED');
  }

  // ---------- AKTIVIRANJE / BRISANJE ----------
  toggle(n: AdminAnnouncement): void {
    this.api.toggle(n.id).subscribe(() => {
      n.isActive = !n.isActive; // odmah osvježi prikaz bez ponovnog učitavanja
      this.toaster.success(this.translate.instant(n.isActive ? 'GAMES.NEWS_ADMIN.ACTIVATED' : 'GAMES.NEWS_ADMIN.DEACTIVATED'));
    });
  }

  remove(n: AdminAnnouncement): void {
    this.dialogHelper
      .confirm('GAMES.NEWS_ADMIN.DELETE_TITLE', 'GAMES.NEWS_ADMIN.DELETE_MESSAGE', { title: n.title })
      .subscribe((res) => {
        if (res?.button !== DialogButton.YES) return;
        this.api.delete(n.id).subscribe(() => {
          this.toaster.success(this.translate.instant('GAMES.NEWS_ADMIN.DELETED'));
          this.load();
        });
      });
  }

  /**
   * Stvarni status u javnom feedu (kombinacija IsActive i termina):
   * inactive | scheduled (još nije objavljena) | expired (istekla) | live (vidljiva)
   */
  feedStatus(n: { isActive: boolean; publishAt: string | null; expiresAt: string | null }): string {
    const now = new Date();
    if (!n.isActive) return 'inactive';
    if (n.publishAt && new Date(n.publishAt) > now) return 'scheduled';
    if (n.expiresAt && new Date(n.expiresAt) <= now) return 'expired';
    return 'live';
  }

  /** Podaci za pregled uživo u modalu */
  get previewData() {
    const v = this.newsForm.getRawValue();
    return { ...v, publishAt: v.publishAt || null, expiresAt: v.expiresAt || null };
  }

  invalid(field: string): boolean {
    const c = this.newsForm.get(field);
    return !!c && c.invalid && c.touched;
  }

  /** Kratak pregled teksta u tabeli (bez HTML tagova) */
  preview(text: string): string {
    const plain = text.replace(/<[^>]*>/g, ' ').replace(/\s+/g, ' ').trim();
    return plain.length > 90 ? plain.slice(0, 90) + '…' : plain;
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
