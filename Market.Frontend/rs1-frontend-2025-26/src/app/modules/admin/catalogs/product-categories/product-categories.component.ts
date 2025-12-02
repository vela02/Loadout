// src/app/modules/admin/catalogs/product-categories/product-categories.component.ts
import { Component, OnInit, inject } from '@angular/core';
import { BaseListComponent } from '../../../../core/components/baseListComponent';
import { ProductCategoriesService } from '../../../../core/services/product-categories/product-categories.service';
import {
  ListProductCategoriesRequest,
  ProductCategoryListItem
} from '../../../../core/services/product-categories/product-categories.model';

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

  displayedColumns: string[] = ['name', 'actions'];

  search: string = '';
  onlyEnabled: boolean = false;

  ngOnInit() {
    this.initList();
  }

  protected loadData() {
    this.startLoading();

    const request: ListProductCategoriesRequest = {
      search: this.search?.trim() || undefined,
      // ako je checkbox/toggle uključen → šaljemo true, ako nije → undefined (da BE ne filtrira)
      onlyEnabled: this.onlyEnabled ? true : undefined,
    };

    this.productCategoriesService.list(request).subscribe({
      next: res => {
        this.items = res;
        this.stopLoading();
      },
      error: () => this.stopLoading('Greška pri učitavanju kategorija')
    });
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.search = value;
    this.loadData();
  }

  onOnlyEnabledChanged(checked: boolean) {
    this.onlyEnabled = checked;
    this.loadData();
  }

  onCreate() {
    // TODO: navigacija ili dijalog
    console.log('create category');
  }

  onEdit(category: ProductCategoryListItem) {
    // TODO: navigacija ili dijalog
    console.log('edit category', category);
  }

  onDelete(category: ProductCategoryListItem) {
    // TODO: confirm + service.delete
    console.log('delete category', category);
  }
}
