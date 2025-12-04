import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  ListProductsRequest,
  ListProductsQueryDto
} from '../../../../api-services/products/products-api.models';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent
  extends BaseListPagedComponent<ListProductsQueryDto, ListProductsRequest>
  implements OnInit {

  private api = inject(ProductsApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);

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
        this.stopLoading('Failed to load products');
        console.error('Load products error:', err);
      }
    });
  }

  // === UI Actions ===

  onCreate(): void {
    this.router.navigate(['/admin/products/add']);
  }

  onEdit(product: ListProductsQueryDto): void {
    this.router.navigate(['/admin/products', product.id, 'edit']);
  }

  onDelete(product: ListProductsQueryDto): void {
    const confirmed = confirm(
      `Are you sure you want to delete product "${product.name}"?`
    );

    if (!confirmed) {
      return;
    }

    this.startLoading();

    this.api.delete(product.id).subscribe({
      next: () => {
        this.toaster.success('Product deleted successfully');
        this.loadPagedData(); // Reload current page
      },
      error: (err) => {
        this.stopLoading('Failed to delete product');
        console.error('Delete product error:', err);
      }
    });
  }

  onSearch(): void {
    this.request.paging.page = 1; // Reset to first page
    this.loadPagedData();
  }
}
