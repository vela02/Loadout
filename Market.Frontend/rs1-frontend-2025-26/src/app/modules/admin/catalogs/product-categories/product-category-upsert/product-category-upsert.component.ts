import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import {
  UpsertProductCategoryCommand,
  ProductCategoryListItem
} from '../../../../../core/services/product-categories/product-categories.model';

export type ProductCategoryDialogMode = 'create' | 'edit';

export interface ProductCategoryDialogData {
  mode: ProductCategoryDialogMode;
  category?: ProductCategoryListItem;
}

export interface ProductCategoryDialogResult {
  mode: ProductCategoryDialogMode;
  id?: number;
  payload: UpsertProductCategoryCommand;
}
@Component({
  selector: 'app-product-category-upsert',
  templateUrl: './product-category-upsert.component.html',
  styleUrls: ['./product-category-upsert.component.scss'],
  standalone: false,
})
export class ProductCategoryUpsertComponent implements OnInit {
  form: FormGroup;
  isSubmitting = false;
  title: string;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ProductCategoryUpsertComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ProductCategoryDialogData
  ) {
    this.title =
      data.mode === 'create' ? 'Nova kategorija' : 'Izmijeni kategoriju';

    // Kreiraj formu
    this.form = this.fb.group({
      name: [
        '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(100),
        ],
      ],
    });
  }

  ngOnInit(): void {
    // Ako je edit mode, popuni formu sa postojećim podacima
    if (this.data.mode === 'edit' && this.data.category) {
      this.form.patchValue({
        name: this.data.category.name,
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.value;
    const upsertDto: UpsertProductCategoryCommand = {
      name: formValue.name.trim(),
    };

    // Vrati podatke roditeljskoj komponenti
    this.dialogRef.close({
      data: upsertDto,
      mode: this.data.mode,
      id: this.data.category?.id,
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  // Helper metode za validaciju
  getErrorMessage(fieldName: string): string {
    const control = this.form.get(fieldName);

    if (control?.hasError('required')) {
      return 'Ovo polje je obavezno';
    }

    if (control?.hasError('minlength')) {
      const minLength = control.errors?.['minlength'].requiredLength;
      return `Minimalno ${minLength} karaktera`;
    }

    if (control?.hasError('maxlength')) {
      const maxLength = control.errors?.['maxlength'].requiredLength;
      return `Maksimalno ${maxLength} karaktera`;
    }

    return '';
  }
}
