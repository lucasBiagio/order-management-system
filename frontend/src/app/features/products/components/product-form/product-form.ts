import { Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

import { Product } from '../../../../core/models/product';
import { ProductService } from '../../../../core/services/product.service';

export interface ProductFormData {
  product?: Product;
}

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss'
})
export class ProductForm {
  private readonly formBuilder = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly dialogRef = inject(MatDialogRef<ProductForm>);

  readonly data = inject<ProductFormData | null>(
    MAT_DIALOG_DATA,
    { optional: true }
  );

  readonly isSaving = signal(false);
  readonly isEditing = Boolean(this.data?.product);

  readonly form = this.formBuilder.nonNullable.group({
    name: [
      this.data?.product?.name ?? '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(120)
      ]
    ],
    price: [
      this.data?.product?.price ?? 0,
      [
        Validators.required,
        Validators.min(0.01)
      ]
    ],
    currentStock: [
      this.data?.product?.currentStock ?? 0,
      [
        Validators.required,
        Validators.min(0)
      ]
    ]
  });

  submit(): void {
    if (this.form.invalid || this.isSaving()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);

    const request = this.form.getRawValue();
    const product = this.data?.product;

    if (product) {
      this.productService.update(product.id, request).subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });

      return;
    }

    this.productService.create(request).subscribe({
      next: () => {
        this.dialogRef.close(true);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}