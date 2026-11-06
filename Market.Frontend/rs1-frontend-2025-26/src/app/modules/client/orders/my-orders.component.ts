import { Component, OnInit, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { GamesApiService } from '../../../api-services/games/games-api.service';
import { OrderHistory } from '../../../api-services/games/games-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';
import { ReceiptPdfService } from '../../../core/services/receipt-pdf.service';

/** Historija kupovina s licencnim ključevima + otkazivanje pre-ordera */
@Component({
  selector: 'app-my-orders',
  standalone: false,
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.scss',
})
export class MyOrdersComponent implements OnInit {
  private gamesApi = inject(GamesApiService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private dialogHelper = inject(DialogHelperService);
  private receiptPdf = inject(ReceiptPdfService);

  /** Ključ za koji je trenutno otvoren QR kod (samo jedan odjednom) */
  qrKey: string | null = null;

  orders: OrderHistory[] = [];
  loading = true;
  cancellingId: number | null = null;

  readonly placeholderImage = 'https://placehold.co/160x90/171a36/8d90b8?text=Loadout';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    const user = this.currentUser.snapshot;
    if (!user) return;
    this.loading = true;
    this.gamesApi.getOrderHistory(user.userId).subscribe({
      next: (orders) => {
        this.orders = orders;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  /** Statusi iz baze: 4 = Pre-order, 5 = Otkazano, ostalo = plaćeno */
  statusClass(o: OrderHistory): string {
    if (o.statusId === 4) return 'status-preorder';
    if (o.statusId === 5) return 'status-cancelled';
    return 'status-paid';
  }

  cancel(order: OrderHistory): void {
    // Velidov zajednički dijalog za potvrdu (Da / Ne)
    this.dialogHelper
      .confirm('GAMES.ORDERS.CANCEL_TITLE', 'GAMES.ORDERS.CANCEL_MESSAGE', { ref: order.referenceNumber ?? order.id })
      .subscribe((result) => {
        if (result?.button !== DialogButton.YES) return;

        this.cancellingId = order.id;
        this.gamesApi.cancelPreOrder(order.id).subscribe({
          next: () => {
            this.cancellingId = null;
            this.toaster.success(this.translate.instant('GAMES.ORDERS.CANCELLED_OK'));
            this.load(); // lista se osvježi bez refresha stranice -> status "Otkazano"
          },
          error: () => {
            this.cancellingId = null;
            this.toaster.error(this.translate.instant('GAMES.ORDERS.CANCEL_FAILED'));
          },
        });
      });
  }

  toggleQr(key: string): void {
    this.qrKey = this.qrKey === key ? null : key;
  }

  /** PDF potvrda o kupovini (parametar: narudžba / ID) */
  downloadPdf(order: OrderHistory): void {
    this.receiptPdf.download(order, this.currentUser.snapshot?.email ?? '');
  }

  onImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    if (img.src !== this.placeholderImage) img.src = this.placeholderImage;
  }
}
