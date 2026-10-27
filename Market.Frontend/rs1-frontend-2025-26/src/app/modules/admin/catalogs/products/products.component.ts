// products.component.ts

import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import {
  ListProductsRequest,
  ListProductsQueryDto
} from '../../../../api-services/products/products-api.models';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ProductCategoriesApiService } from '../../../../api-services/product-categories/product-categories-api.service';
import { ListProductCategoriesQueryDto } from '../../../../api-services/product-categories/product-categories-api.model';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';
import {
  PROMOTION_DIALOG_CONFIG,
  PromotionCreateDialogComponent
} from '../promotions/promotion-create-dialog/promotion-create-dialog.component';

type StatusFilter = 'all' | 'active' | 'inactive';

/** Min price must not be greater than max price */
const priceRangeValidator: ValidatorFn = (group: AbstractControl): ValidationErrors | null => {
  const min = group.get('minPrice')?.value;
  const max = group.get('maxPrice')?.value;
  return min != null && max != null && min > max ? { priceRange: true } : null;
};

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent
  extends BaseListPagedComponent<ListProductsQueryDto, ListProductsRequest>
  implements OnInit {

  private api = inject(ProductsApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private dialog = inject(MatDialog);
  private categoriesApi = inject(ProductCategoriesApiService);
  private fb = inject(FormBuilder);

  categories: ListProductCategoriesQueryDto[] = [];

  /** 5 filters: search, category, status, min price, max price */
  filterForm = this.fb.group(
    {
      search: ['', Validators.maxLength(100)],
      categoryId: [null as number | null],
      status: ['all' as StatusFilter],
      minPrice: [null as number | null, Validators.min(0)],
      maxPrice: [null as number | null, Validators.min(0)]
    },
    { validators: priceRangeValidator }
  );

  /** Number of filters applied to the current list (shown as a badge) */
  activeFilterCount = 0;

  sortState: {
    column: 'name' | 'categoryName' | 'price' | 'isEnabled' | null;
    direction: 'asc' | 'desc';
  } = {
    column: null,
    direction: 'asc'
  };

  displayedColumns: string[] = [
    'name',
    'categoryName',
    'price',
    'stockQuantity',
    'isEnabled',
    'actions'
  ];

  constructor() {
    super();
    this.request = new ListProductsRequest();
    this.request.paging.pageSize = 10;
  }

  ngOnInit(): void {
    this.loadCategories();
    this.initList();
  }

  private loadCategories(): void {
    this.categoriesApi.list({ paging: { page: 1, pageSize: 1000 } }).subscribe({
      next: (res) => (this.categories = res.items),
      error: (err) => console.error('Load categories error:', err)
    });
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load products');
        console.error('Load products error:', err);
      }
    });
  }

  // === UI Actions ===

  onCreate(): void {
    this.router.navigate(['/admin/products/add']);
  }

  onCreatePromotion(): void {
    this.dialog
      .open(PromotionCreateDialogComponent, PROMOTION_DIALOG_CONFIG)
      .afterClosed()
      .subscribe((created: boolean) => {
        // reload so the new sale prices show up
        if (created) this.loadPagedData();
      });
  }

  onEdit(product: ListProductsQueryDto): void {
    this.router.navigate(['/admin/products', product.id, 'edit']);
  }

  onDelete(product: ListProductsQueryDto): void {
    this.dialogHelper.product.confirmDelete(product.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(product);
      }
    });
  }

  private performDelete(product: ListProductsQueryDto): void {
    this.startLoading();

    this.api.delete(product.id).subscribe({
      next: () => {
        this.dialogHelper.product.showDeleteSuccess().subscribe();
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'PRODUCTS.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete product error:', err);
      }
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
    this.request.categoryId = f.categoryId ?? null;
    this.request.isEnabled = f.status === 'all' ? null : f.status === 'active';
    this.request.minPrice = f.minPrice ?? null;
    this.request.maxPrice = f.maxPrice ?? null;

    this.activeFilterCount = [
      this.request.search,
      this.request.categoryId,
      this.request.isEnabled,
      this.request.minPrice,
      this.request.maxPrice
    ].filter(v => v !== null && v !== undefined).length;

    this.request.paging.page = 1;
    this.loadPagedData();
  }

  resetFilters(): void {
    this.filterForm.reset({ search: '', categoryId: null, status: 'all', minPrice: null, maxPrice: null });
    this.applyFilters();
  }

  priceError(controlName: 'minPrice' | 'maxPrice'): string | null {
    const control = this.filterForm.get(controlName);
    if (control?.hasError('min')) return 'PRODUCTS.FILTERS.PRICE_NEGATIVE';
    if (this.filterForm.hasError('priceRange')) return 'PRODUCTS.FILTERS.PRICE_RANGE';
    return null;
  }

  sortBy(column: 'name' | 'categoryName' | 'price' | 'isEnabled'): void {
    if (this.sortState.column === column) {
      this.sortState.direction = this.sortState.direction === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortState.column = column;
      this.sortState.direction = 'asc';
    }

    this.applyClientSort();
  }

  private applyClientSort(): void {
    if (!this.sortState.column) {
      return;
    }

    const direction = this.sortState.direction === 'asc' ? 1 : -1;

    this.items = [...this.items].sort((a, b) => {
      const valueA = this.getSortValue(a, this.sortState.column!);
      const valueB = this.getSortValue(b, this.sortState.column!);

      if (typeof valueA === 'string' && typeof valueB === 'string') {
        return valueA.localeCompare(valueB) * direction;
      }

      return ((Number(valueA) || 0) - (Number(valueB) || 0)) * direction;
    });
  }

  private getSortValue(
    item: ListProductsQueryDto,
    column: 'name' | 'categoryName' | 'price' | 'isEnabled'
  ): string | number {
    switch (column) {
      case 'name':
        return item.name ?? '';
      case 'categoryName':
        return item.categoryName ?? '';
      case 'price':
        // sort by the price shown in the table (after discount)
        return (item.price ?? 0) * (1 - (item.activeDiscountPercentage ?? 0) / 100);
      case 'isEnabled':
        return item.isEnabled ? 1 : 0;
      default:
        return 0;
    }
  }
}
