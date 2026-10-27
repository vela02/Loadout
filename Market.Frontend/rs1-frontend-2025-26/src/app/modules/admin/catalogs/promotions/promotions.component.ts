import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import { PromotionsApiService } from '../../../../api-services/promotions/promotions-api.service';
import { ListPromotionsQueryDto } from '../../../../api-services/promotions/promotions-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  PROMOTION_DIALOG_CONFIG,
  PromotionCreateDialogComponent
} from './promotion-create-dialog/promotion-create-dialog.component';

export type PromotionStatus = 'ACTIVE' | 'SCHEDULED' | 'EXPIRED';

@Component({
  selector: 'app-promotions',
  standalone: false,
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.scss'
})
export class PromotionsComponent extends BaseListComponent<ListPromotionsQueryDto> implements OnInit {
  private api = inject(PromotionsApiService);
  private dialog = inject(MatDialog);
  private dialogHelper = inject(DialogHelperService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  displayedColumns: string[] = ['name', 'target', 'discount', 'period', 'games', 'status', 'actions'];

  ngOnInit(): void {
    this.initList();
  }

  protected loadData(): void {
    this.startLoading();

    this.api.list().subscribe({
      next: (promotions) => {
        this.items = promotions;
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading(this.translate.instant('PROMOTIONS.DIALOGS.ERROR_LOAD'));
        console.error('Load promotions error:', err);
      }
    });
  }

  getStatus(promotion: ListPromotionsQueryDto): PromotionStatus {
    const now = new Date();
    if (new Date(promotion.endDate) < now) return 'EXPIRED';
    if (new Date(promotion.startDate) > now) return 'SCHEDULED';
    return 'ACTIVE';
  }

  countByStatus(status: PromotionStatus): number {
    return this.items.filter(p => this.getStatus(p) === status).length;
  }

  // === UI Actions ===

  onCreate(): void {
    this.dialog
      .open(PromotionCreateDialogComponent, PROMOTION_DIALOG_CONFIG)
      .afterClosed()
      .subscribe((created: boolean) => {
        if (created) this.loadData();
      });
  }

  onDelete(promotion: ListPromotionsQueryDto): void {
    this.dialogHelper
      .confirmDelete(promotion.name, 'PROMOTIONS.DIALOGS.DELETE_MESSAGE')
      .subscribe(result => {
        if (result && result.button === DialogButton.DELETE) {
          this.performDelete(promotion);
        }
      });
  }

  private performDelete(promotion: ListPromotionsQueryDto): void {
    this.api.delete(promotion.id).subscribe({
      next: () => {
        this.toaster.success(this.translate.instant('PROMOTIONS.DIALOGS.SUCCESS_DELETE'));
        this.loadData();
      },
      error: (err) => {
        this.dialogHelper.showError('DIALOGS.TITLES.ERROR', 'PROMOTIONS.DIALOGS.ERROR_DELETE').subscribe();
        console.error('Delete promotion error:', err);
      }
    });
  }
}
