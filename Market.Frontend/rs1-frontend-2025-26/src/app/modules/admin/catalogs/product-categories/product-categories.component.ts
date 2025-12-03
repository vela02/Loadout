import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { BaseListComponent } from '../../../../core/components/baseListComponent';
import {
  ListProductCategoriesRequest,
  ProductCategoryListItem,
  UpsertProductCategoryCommand,
} from '../../../../core/services/product-categories/product-categories.model';
import { ProductCategoriesService } from '../../../../core/services/product-categories/product-categories.service';
import { ProductCategoryUpsertComponent } from './product-category-upsert/product-category-upsert.component';

interface ProductCategoryDialogResult {
  id?: number;
  data: UpsertProductCategoryCommand;
}

@Component({
  selector: 'app-product-categories',
  standalone: false,
  templateUrl: './product-categories.component.html',
  styleUrl: './product-categories.component.scss',
})
export class ProductCategoriesComponent
  extends BaseListComponent<ProductCategoryListItem>
  implements OnInit {

  private productCategoriesService = inject(ProductCategoriesService);
  private dialog = inject(MatDialog);

  displayedColumns: string[] = ['name', 'actions'];

  request : ListProductCategoriesRequest = {
    search :"",
    onlyEnabled : true,
    paging: { page:1, pageSize: 100 }
  }

  ngOnInit(): void {
    this.initList();
  }

  // === Učitavanje liste ===
  protected loadData(): void {
    this.startLoading();
    this.productCategoriesService.list(this.request).subscribe({
      next: res => {
        this.items = res.items;
        this.stopLoading();
      },
      error: () => this.stopLoading('Greška pri učitavanju kategorija'),
    });
  }

  // === Filteri ===
  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.request.search = value;
    this.loadData();
  }

  onOnlyEnabledChanged(checked: boolean): void {
    this.request.onlyEnabled = checked;
    this.loadData();
  }

  // === Kreiranje ===
  onCreate(): void {
    const dialogRef = this.dialog.open<ProductCategoryUpsertComponent, any, ProductCategoryDialogResult>(
      ProductCategoryUpsertComponent,
      {
        width: '500px',
        disableClose: true,
        data: {
          mode: 'create',
        },
      }
    );

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.startLoading();

      this.productCategoriesService.create(result.data).subscribe({
        next: () => {
          this.loadData();
        },
        error: () => this.stopLoading('Greška pri kreiranju kategorije'),
      });
    });
  }

  // === Uređivanje ===
  onEdit(category: ProductCategoryListItem): void {
    const dialogRef = this.dialog.open<ProductCategoryUpsertComponent, any, ProductCategoryDialogResult>(
      ProductCategoryUpsertComponent,
      {
        width: '500px',
        disableClose: true,
        data: {
          mode: 'edit',
          category,
        },
      }
    );

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.startLoading();

      this.productCategoriesService.update(category.id, result.data).subscribe({
        next: () => {
          this.loadData();
        },
        error: () => this.stopLoading('Greška pri izmjeni kategorije'),
      });
    });
  }

  // === Brisanje ===
  onDelete(category: ProductCategoryListItem): void {
    const confirmed = confirm(
      `Da li ste sigurni da želite obrisati kategoriju "${category.name}"?`
    );

    if (!confirmed) {
      return;
    }

    this.startLoading();

    this.productCategoriesService.delete(category.id).subscribe({
      next: () => {
        this.loadData();
      },
      error: () => this.stopLoading('Greška pri brisanju kategorije'),
    });
  }
}
