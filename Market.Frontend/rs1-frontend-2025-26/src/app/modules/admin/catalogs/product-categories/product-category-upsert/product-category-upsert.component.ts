import { Component, inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ProductCategoriesApiService } from '../../../../../api-services/product-categories/product-categories-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';
import {
  GetProductCategoryByIdQueryDto,
  CreateProductCategoryCommand,
  UpdateProductCategoryCommand
} from '../../../../../api-services/product-categories/product-categories-api.model';
import {ProductCategoryFormService} from "../services/product-category-form.service";

export interface ProductCategoryDialogData {
  mode: 'create' | 'edit';
  categoryId?: number;
}

@Component({
  selector: 'app-product-category-upsert',
  standalone: false,
  templateUrl: './product-category-upsert.component.html',
  styleUrl: './product-category-upsert.component.scss',
  providers: [ProductCategoryFormService]
})
export class ProductCategoryUpsertComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<ProductCategoryUpsertComponent>);
  private data = inject<ProductCategoryDialogData>(MAT_DIALOG_DATA);
  private api = inject(ProductCategoriesApiService);
  private formService = inject(ProductCategoryFormService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  form!: FormGroup;
  isLoading = false;
  isEditMode = false;

  /** Translation key for the dialog title */
  title = '';

  ngOnInit(): void {
    this.isEditMode = this.data.mode === 'edit';
    this.title = this.isEditMode ? 'PRODUCT_CATEGORIES.FORM.EDIT_TITLE' : 'PRODUCT_CATEGORIES.NEW_CATEGORY';

    if (this.isEditMode && this.data.categoryId) {
      this.loadCategory(this.data.categoryId);
    } else {
      this.form = this.formService.createCategoryForm();
    }
  }

  private loadCategory(id: number): void {
    this.isLoading = true;

    this.api.getById(id).subscribe({
      next: (category) => {
        this.form = this.formService.createCategoryForm(category);
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error(this.translate.instant('PRODUCT_CATEGORIES.FORM.ERROR_LOAD'));
        console.error('Load category error:', err);
        this.dialogRef.close(false);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    if (this.isEditMode && this.data.categoryId) {
      this.updateCategory();
    } else {
      this.createCategory();
    }
  }

  private createCategory(): void {
    const command: CreateProductCategoryCommand = {
      name: this.form.value.name.trim()
    };

    this.api.create(command).subscribe({
      next: () => {
        this.dialogRef.close(true); // Signal success (the page shows the success dialog)
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Create category error:', err);
      }
    });
  }

  private updateCategory(): void {
    const command: UpdateProductCategoryCommand = {
      name: this.form.value.name.trim()
    };

    this.api.update(this.data.categoryId!, command).subscribe({
      next: () => {
        this.dialogRef.close(true); // Signal success (the page shows the success dialog)
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Update category error:', err);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
