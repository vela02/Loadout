import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import {
  AdminUser, AdminUsersApiService, PagedResult, Role, UserFilter,
} from '../../../api-services/admin-users/admin-users-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';

/**
 * ADMIN – UPRAVLJANJE KORISNICIMA (CRUD)
 *  - filter sa 5 parametara (pretraga, uloga, status, registrovan od/do)
 *  - sortiranje klikom na zaglavlje kolone
 *  - backend paging
 *  - dodavanje / izmjena u modalnoj formi s FE validacijom (BE validacija je u UserUpdateDto)
 *  - brisanje (soft delete) uz potvrdu
 */
@Component({
  selector: 'app-admin-users',
  standalone: false,
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit, OnDestroy {
  private api = inject(AdminUsersApiService);
  private fb = inject(FormBuilder);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private dialogHelper = inject(DialogHelperService);

  roles: Role[] = [];
  result: PagedResult<AdminUser> | null = null;
  loading = false;

  sortBy = 'createdAt';
  sortDir: 'asc' | 'desc' = 'desc';
  page = 1;
  pageSize = 10;
  readonly pageSizes = [5, 10, 20, 50];

  // ---------- FILTER FORMA (5 parametara) ----------
  filterForm = this.fb.group({
    search: [''],
    roleId: [null as number | null],
    isEnabled: [null as boolean | null],
    createdFrom: [''],
    createdTo: [''],
  });

  // ---------- FORMA ZA DODAVANJE / IZMJENU ----------
  editingId: number | null = null;   // null = novi korisnik
  showForm = false;
  saving = false;
  serverError = '';

  userForm = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50), Validators.pattern(/^[A-Za-z0-9._-]+$/)]],
    email: ['', [Validators.required, Validators.email, Validators.pattern(/^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/)]],
    roleId: [0, [Validators.required, Validators.min(1)]],
    isEnabled: [true],
    password: ['', [Validators.minLength(6)]],
  });

  private sub?: Subscription;

  ngOnInit(): void {
    this.api.getRoles().subscribe((r) => (this.roles = r));
    this.load();

    // Svaka promjena filtera -> nazad na 1. stranicu i ponovno učitavanje (debounce za kucanje)
    this.sub = this.filterForm.valueChanges.pipe(debounceTime(350)).subscribe(() => {
      this.page = 1;
      this.load();
    });
  }

  load(): void {
    const f = this.filterForm.getRawValue();
    const filter: UserFilter = {
      search: f.search,
      roleId: f.roleId,
      isEnabled: f.isEnabled,
      createdFrom: f.createdFrom,
      createdTo: f.createdTo,
      sortBy: this.sortBy,
      sortDir: this.sortDir,
      page: this.page,
      pageSize: this.pageSize,
    };
    this.loading = true;
    this.api.getPaged(filter).subscribe({
      next: (r) => {
        this.result = r;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  resetFilters(): void {
    this.filterForm.reset({ search: '', roleId: null, isEnabled: null, createdFrom: '', createdTo: '' });
  }

  get activeFilterCount(): number {
    const f = this.filterForm.getRawValue();
    return [f.search, f.roleId, f.isEnabled, f.createdFrom, f.createdTo]
      .filter((v) => v !== null && v !== '').length;
  }

  // ---------- SORTIRANJE ----------
  sort(column: string): void {
    if (this.sortBy === column) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc'; // drugi klik = obrnut smjer
    } else {
      this.sortBy = column;
      this.sortDir = 'asc';
    }
    this.load();
  }

  sortIcon(column: string): string {
    if (this.sortBy !== column) return 'unfold_more';
    return this.sortDir === 'asc' ? 'arrow_upward' : 'arrow_downward';
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
    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }

  // ---------- DODAVANJE / IZMJENA ----------
  openCreate(): void {
    this.editingId = null;
    this.serverError = '';
    this.userForm.reset({ username: '', email: '', roleId: 0, isEnabled: true, password: '' });
    // kod novog korisnika šifra je obavezna
    this.userForm.controls.password.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.controls.password.updateValueAndValidity();
    this.showForm = true;
  }

  openEdit(u: AdminUser): void {
    this.editingId = u.id;
    this.serverError = '';
    this.userForm.reset({ username: u.username, email: u.email, roleId: u.roleId, isEnabled: u.isEnabled, password: '' });
    // kod izmjene šifra je opcionalna (prazno = ne mijenja se)
    this.userForm.controls.password.setValidators([Validators.minLength(6)]);
    this.userForm.controls.password.updateValueAndValidity();
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
  }

  save(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }
    const v = this.userForm.getRawValue();
    const dto = { ...v, roleId: Number(v.roleId), password: v.password || null };

    this.saving = true;
    this.serverError = '';
    const request = this.editingId ? this.api.update(this.editingId, dto) : this.api.create(dto);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.showForm = false;
        this.toaster.success(this.translate.instant(this.editingId ? 'GAMES.USERS.UPDATED' : 'GAMES.USERS.CREATED_OK'));
        this.load();
      },
      error: (err: HttpErrorResponse) => {
        this.saving = false;
        // 409 = email/username zauzet (backend provjera), 400 = backend validacija
        this.serverError = err.error?.message || err.error?.title || this.translate.instant('GAMES.USERS.SAVE_FAILED');
      },
    });
  }

  // ---------- BRISANJE ----------
  remove(u: AdminUser): void {
    this.dialogHelper
      .confirm('GAMES.USERS.DELETE_TITLE', 'GAMES.USERS.DELETE_MESSAGE', { name: u.username })
      .subscribe((res) => {
        if (res?.button !== DialogButton.YES) return;
        this.api.delete(u.id).subscribe(() => {
          this.toaster.success(this.translate.instant('GAMES.USERS.DELETED'));
          this.load();
        });
      });
  }

  invalid(field: string): boolean {
    const c = this.userForm.get(field);
    return !!c && c.invalid && c.touched;
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}
