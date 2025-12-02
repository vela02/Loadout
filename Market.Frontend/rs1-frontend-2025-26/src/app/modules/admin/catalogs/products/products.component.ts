// src/app/modules/admin/catalogs/products/products.component.ts

import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import {
  ListProductsRequest,
  ProductListItem,
} from '../../../../core/services/products/products.models';
import { ProductsService } from '../../../../core/services/products/products.service';
import { BasePagedComponent } from '../../../../core/components/basePagedComponent';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss',
})
export class ProductsComponent
  extends BasePagedComponent<ProductListItem, ListProductsRequest>
  implements OnInit {

  private productsService = inject(ProductsService);
  private router = inject(Router);

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

    this.productsService.list(this.request).subscribe({
      next: res => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading('Greška pri učitavanju proizvoda'),
    });
  }

  // === UI actions ===

  onCreate(): void {
    this.router.navigate(['/admin/products/add']);
  }

  onEdit(product: ProductListItem): void {
    this.router.navigate(['/admin/products', product.id, 'edit']);
  }

  onDelete(product: ProductListItem): void {
    const confirmed = confirm(
      `Da li ste sigurni da želite obrisati proizvod "${product.name}"?`
    );
    if (!confirmed) {
      return;
    }

    this.startLoading();

    this.productsService.delete(product.id).subscribe({
      next: () => {
        // nakon brisanja samo ponovo učitaj trenutnu stranicu
        this.loadPagedData();
      },
      error: () => this.stopLoading('Greška pri brisanju proizvoda'),
    });
  }
}
