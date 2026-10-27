import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { BaseFormComponent } from '../../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ProductCategoriesApiService } from '../../../../api-services/product-categories/product-categories-api.service';
import { CreateProductCommand, UpdateProductCommand, GetProductByIdQueryDto } from '../../../../api-services/products/products-api.models';
import { ListProductCategoriesQueryDto } from '../../../../api-services/product-categories/product-categories-api.model';
import { SharedModule } from "../../../shared/shared-module";
import { TranslateService } from '@ngx-translate/core';


interface ProductFormModel {
  id?: number;                
  name: string;
  description?: string | null; 
  price: number;
  categoryId: number;
}

@Component({
  selector: 'app-custom-products-add',
  templateUrl: './custom-products-add.component.html',
  styleUrls: ['./custom-products-add.component.scss'],
  standalone: false,
  
})
export class CustomProductsAddComponent extends BaseFormComponent<ProductFormModel> implements OnInit {
  private fb = inject(FormBuilder);
  private productsApi = inject(ProductsApiService);
  private categoriesApi = inject(ProductCategoriesApiService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private translate = inject(TranslateService);

 categories$: Observable<ListProductCategoriesQueryDto[]> = this.categoriesApi.list({ 
    onlyEnabled: true,
    paging: { page: 1, pageSize: 100 } 
  }).pipe(
    map(response => response.items)
  );

  ngOnInit(): void {
    this.buildForm();
    this.initForm(this.isEditMode);
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      price: [0, [Validators.required, Validators.min(0.01)]],
      categoryId: [null, [Validators.required]]
    });
  }

  protected loadData(): void {
    const id = +this.route.snapshot.params['id'];
    if (id) {
      this.productsApi.getById(id).subscribe({
        next: (product) => {
          this.model = product;
          this.form.patchValue({
            name: product.name,
            description: product.description || '',
            price: product.price,
            categoryId: product.categoryId
          });
        },
        error: () => {
          this.toaster.error(this.translate.instant('PRODUCTS.DIALOGS.ERROR_UPDATE'));
          this.router.navigate(['/admin/products']);
        }
      });
    }
  }

 protected save(): void {
    const formValue = this.form.value as ProductFormModel;

    // Spremamo podatke iz forme u command objekat
    const command: CreateProductCommand = {
      title: formValue.name,
      description: formValue.description || null,
      price: formValue.price,
      categoryId: formValue.categoryId
    };

    if (this.isEditMode) {
      // --- UPDATE LOGIKA ---
      // Uzimamo ID iz rute, plus (+) ga pretvara u striktni number
      const id = +this.route.snapshot.params['id']; 
      
      this.productsApi.update(id, command as UpdateProductCommand).subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('PRODUCTS.DIALOGS.SUCCESS_UPDATE'));
          this.router.navigate(['/admin/products']);
        },
        error: () => {
          this.toaster.error(this.translate.instant('PRODUCTS.DIALOGS.ERROR_UPDATE'));
        }
      });
    } else {
      // --- CREATE LOGIKA ---
      this.productsApi.create(command).subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('PRODUCTS.DIALOGS.SUCCESS_CREATE'));
          this.router.navigate(['/admin/products']);
        },
        error: () => {
          this.toaster.error(this.translate.instant('PRODUCTS.DIALOGS.ERROR_CREATE'));
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/admin/products']);
  }
}
