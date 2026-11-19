import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { AdminUsersApiService, UserPurchaseHistory } from '../../../../api-services/admin-users/admin-users-api.service';
import { ReceiptPdfService } from '../../../../core/services/receipt-pdf.service';
import { ToasterService } from '../../../../core/services/toaster.service';

/**
 * ADMIN – HISTORIJA KUPOVINA KORISNIKA (/admin/users/:id/history)
 *  - podaci o korisniku i statistika (broj narudžbi, igara, ukupno potrošeno)
 *  - filter po periodu (od–do) s validacijom
 *  - PDF izvještaj s parametrima: korisnik, datum od, datum do
 */
@Component({
  selector: 'app-admin-user-history',
  standalone: false,
  templateUrl: './admin-user-history.component.html',
  styleUrl: './admin-user-history.component.scss',
})
export class AdminUserHistoryComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private api = inject(AdminUsersApiService);
  private fb = inject(FormBuilder);
  private pdf = inject(ReceiptPdfService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  userId = 0;
  data: UserPurchaseHistory | null = null;
  loading = true;
  notFound = false;
  generating = false;

  readonly placeholderImage = 'https://placehold.co/160x90/171a36/8d90b8?text=Loadout';

  periodForm = this.fb.nonNullable.group({ from: [''], to: [''] });

  /** Validacija perioda: "od" ne smije biti poslije "do" ("YYYY-MM-DD" se ispravno poredi kao tekst) */
  get periodInvalid(): boolean {
    const { from, to } = this.periodForm.getRawValue();
    return !!from && !!to && from > to;
  }

  ngOnInit(): void {
    this.userId = Number(this.route.snapshot.paramMap.get('id'));
    this.load();
    this.periodForm.valueChanges.subscribe(() => {
      if (!this.periodInvalid) this.load();
    });
  }

  load(): void {
    const { from, to } = this.periodForm.getRawValue();
    this.loading = true;
    this.api.getPurchases(this.userId, from, to).subscribe({
      next: (d) => {
        this.data = d;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.notFound = err.status === 404;
      },
    });
  }

  /** Brzi izbor perioda */
  setPreset(preset: 'month' | 'year' | 'all'): void {
    const iso = (d: Date) => d.toISOString().slice(0, 10);
    const today = new Date();
    if (preset === 'all') {
      this.periodForm.setValue({ from: '', to: '' });
      return;
    }
    const start = new Date(today);
    if (preset === 'month') start.setMonth(today.getMonth() - 1);
    else start.setFullYear(today.getFullYear() - 1);
    this.periodForm.setValue({ from: iso(start), to: iso(today) });
  }

  /** PDF izvještaj za trenutno prikazani period */
  async downloadReport(): Promise<void> {
    if (!this.data || this.periodInvalid) return;
    this.generating = true;
    try {
      await this.pdf.downloadUserReport(this.data);
      this.toaster.success(this.translate.instant('GAMES.USER_HISTORY.PDF_OK'));
    } finally {
      this.generating = false;
    }
  }

  statusClass(statusId: number | null): string {
    if (statusId === 4) return 'status-preorder';
    if (statusId === 5) return 'status-cancelled';
    return 'status-paid';
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }
}
