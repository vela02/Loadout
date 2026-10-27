import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ProductCategoriesApiService } from '../../../../api-services/product-categories/product-categories-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  ListProductCategoriesRequest,
  ListProductCategoriesQueryDto,
} from '../../../../api-services/product-categories/product-categories-api.model';
import { ProductCategoryUpsertComponent } from './product-category-upsert/product-category-upsert.component';

type TriStateFilter = 'all' | 'yes' | 'no';

/** Min games must not be greater than max games */
const gamesRangeValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const min = group.get('minGames')?.value;
  const max = group.get('maxGames')?.value;
  return min != null && max != null && min > max ? { gamesRange: true } : null;
};

const toBool = (v: TriStateFilter): boolean | null => (v === 'all' ? null : v === 'yes');

@Component({
  selector: 'app-product-categories',
  standalone: false,
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss',
})
export class ProductCategoriesComponent
  extends BaseListPagedComponent<ListProductCategoriesQueryDto, ListProductCategoriesRequest>
  implements OnInit
{
  private api = inject(ProductCategoriesApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private translate = inject(TranslateService);
  private fb = inject(FormBuilder);

  displayedColumns: string[] = ['name', 'gamesCount', 'onSale', 'isEnabled', 'actions'];

  /** 5 filters: search, status, min games, max games, on sale */
  filterForm = this.fb.group(
    {
      search: ['', Validators.maxLength(50)],
      status: ['all' as TriStateFilter],
      minGames: [null as number | null, [Validators.min(0), Validators.pattern(/^\d+$/)]],
      maxGames: [null as number | null, [Validators.min(0), Validators.pattern(/^\d+$/)]],
      onSale: ['all' as TriStateFilter]
    },
    { validators: gamesRangeValidator }
  );

  /** Number of filters applied to the current list (shown as a badge) */
  activeFilterCount = 0;

  constructor() {
    super();
    this.request = new ListProductCategoriesRequest();
    this.request.paging.pageSize = 10;
  }

  ngOnInit(): void {
    this.initList();
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load categories');
        console.error('Load categories error:', err);
      },
    });
  }

  // === Filters ===

  applyFilters(): void {
    if (this.filterForm.invalid) {
      this.filterForm.markAllAsTouched();
      return;
    }

    const f = this.filterForm.getRawValue();

    this.request.search = f.search?.trim() || null;
    this.request.isEnabled = toBool(f.status ?? 'all');
    this.request.minGames = f.minGames ?? null;
    this.request.maxGames = f.maxGames ?? null;
    this.request.onSale = toBool(f.onSale ?? 'all');

    this.activeFilterCount = [
      this.request.search,
      this.request.isEnabled,
      this.request.minGames,
      this.request.maxGames,
      this.request.onSale
    ].filter(v => v !== null && v !== undefined).length;

    this.request.paging.page = 1;
    this.loadPagedData();
  }

  resetFilters(): void {
    this.filterForm.reset({ search: '', status: 'all', minGames: null, maxGames: null, onSale: 'all' });
    this.applyFilters();
  }

  gamesError(controlName: 'minGames' | 'maxGames'): string | null {
    const control = this.filterForm.get(controlName);
    if (control?.hasError('min')) return 'PRODUCT_CATEGORIES.FILTERS.GAMES_NEGATIVE';
    if (control?.hasError('pattern')) return 'PRODUCT_CATEGORIES.FILTERS.GAMES_WHOLE';
    if (this.filterForm.hasError('gamesRange')) return 'PRODUCT_CATEGORIES.FILTERS.GAMES_RANGE';
    return null;
  }

  // === CRUD Actions ===

  onCreate(): void {
    const dialogRef = this.dialog.open(ProductCategoryUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: ['product-category-dialog', 'loadout-dialog'],
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'create',
      },
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.productCategory.showCreateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onEdit(category: ListProductCategoriesQueryDto): void {
    const dialogRef = this.dialog.open(ProductCategoryUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: ['product-category-dialog', 'loadout-dialog'],
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'edit',
        categoryId: category.id,
      },
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.productCategory.showUpdateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onDelete(category: ListProductCategoriesQueryDto): void {
    this.dialogHelper.productCategory.confirmDelete(category.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(category);
      }
    });
  }

  private performDelete(category: ListProductCategoriesQueryDto): void {
    this.startLoading();

    this.api.delete(category.id).subscribe({
      next: () => {
        this.dialogHelper.productCategory.showDeleteSuccess().subscribe();
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        const errorMessage = this.extractErrorMessage(err);

        // Show error dialog instead of toast
        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'PRODUCT_CATEGORIES.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete category error:', err);
      },
    });
  }

  onToggleStatus(category: ListProductCategoriesQueryDto): void {
    this.startLoading();

    const apiAction = category.isEnabled
      ? this.api.disable(category.id)
      : this.api.enable(category.id);

    apiAction.subscribe({
      next: () => {
        const messageKey = category.isEnabled
          ? 'PRODUCT_CATEGORIES.DIALOGS.SUCCESS_DISABLE'
          : 'PRODUCT_CATEGORIES.DIALOGS.SUCCESS_ENABLE';
        this.toaster.success(this.translate.instant(messageKey, { name: category.name }));
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        const errorMessage = this.extractErrorMessage(err);

        if (err.status === 409) {
          // Business rule conflict - show dialog for important errors
          this.dialogHelper.showWarning(
            'DIALOGS.TITLES.WARNING',
            errorMessage || 'PRODUCT_CATEGORIES.DIALOGS.ERROR_TOGGLE',
            { name: category.name }
          ).subscribe();
        } else {
          // Generic error - just toast
          this.toaster.error(
            errorMessage ||
              this.translate.instant('PRODUCT_CATEGORIES.DIALOGS.ERROR_TOGGLE', { name: category.name })
          );
        }

        console.error('Toggle status error:', err);
        this.loadPagedData();
      },
    });
  }

  /**
   * Extract user-friendly error message from HTTP error response
   */
  private extractErrorMessage(err: any): string | null {
    if (err?.error) {
      if (typeof err.error === 'string') {
        return err.error;
      }

      if (err.error.message) {
        return err.error.message;
      }

      if (err.error.title) {
        return err.error.title;
      }

      if (err.error.errors && typeof err.error.errors === 'object') {
        const errors = Object.values(err.error.errors).flat();
        if (errors.length > 0) {
          return errors.join(', ');
        }
      }
    }

    if (err?.message) {
      return err.message;
    }

    if (err?.statusText && err.statusText !== 'Unknown Error') {
      return err.statusText;
    }

    return null;
  }
}
