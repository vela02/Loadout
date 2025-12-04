import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BasePagedComponent } from '../../../../core/components/basePagedComponent';
import { ProductCategoriesApiService } from '../../../../api-services/product-categories/product-categories-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import {
  ListProductCategoriesRequest,
  ListProductCategoriesQueryDto
} from '../../../../api-services/product-categories/product-categories-api.model';
import { ProductCategoryUpsertComponent } from './product-category-upsert/product-category-upsert.component';

@Component({
  selector: 'app-product-categories',
  standalone: false,
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss'
})
export class ProductCategoriesComponent
  extends BasePagedComponent<ListProductCategoriesQueryDto, ListProductCategoriesRequest>
  implements OnInit {

  private api = inject(ProductCategoriesApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);

  displayedColumns: string[] = ['name', 'isEnabled', 'actions'];
  showOnlyEnabled = true;

  constructor() {
    super();
    this.request = new ListProductCategoriesRequest();
    this.request.onlyEnabled = true;
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
      }
    });
  }

  // === Filters ===

  onSearchChange(searchTerm: string): void {
    this.request.search = searchTerm;
    this.request.paging.page = 1; // Reset to first page
    this.loadPagedData();
  }

  onToggleEnabledFilter(checked: boolean): void {
    this.showOnlyEnabled = checked;
    this.request.onlyEnabled = checked;
    this.request.paging.page = 1; // Reset to first page
    this.loadPagedData();
  }

  // === CRUD Actions ===

  onCreate(): void {
    const dialogRef = this.dialog.open(ProductCategoryUpsertComponent, {
      width: '500px',
      disableClose: true,
      data: {
        mode: 'create'
      }
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.loadPagedData(); // Reload list
      }
    });
  }

  onEdit(category: ListProductCategoriesQueryDto): void {
    const dialogRef = this.dialog.open(ProductCategoryUpsertComponent, {
      width: '500px',
      disableClose: true,
      data: {
        mode: 'edit',
        categoryId: category.id
      }
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.loadPagedData(); // Reload list
      }
    });
  }

  onDelete(category: ListProductCategoriesQueryDto): void {
    const confirmed = confirm(
      `Are you sure you want to delete category "${category.name}"?`
    );

    if (!confirmed) {
      return;
    }

    this.startLoading();

    this.api.delete(category.id).subscribe({
      next: () => {
        this.toaster.success('Category deleted successfully');
        this.loadPagedData(); // Reload current page
      },
      error: (err) => {
        this.stopLoading('Failed to delete category');
        console.error('Delete category error:', err);
      }
    });
  }

  onToggleStatus(category: ListProductCategoriesQueryDto): void {
    this.startLoading();

    const action = category.isEnabled
      ? this.api.disable(category.id)
      : this.api.enable(category.id);

    action.subscribe({
      next: () => {
        const status = category.isEnabled ? 'disabled' : 'enabled';
        this.toaster.success(`Category ${status} successfully`);
        this.loadPagedData(); // Reload to show updated status
      },
      error: (err) => {
        this.stopLoading('Failed to update category status');
        console.error('Toggle status error:', err);
      }
    });
  }
}
