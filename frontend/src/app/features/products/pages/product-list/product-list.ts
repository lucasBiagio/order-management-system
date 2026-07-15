import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { Product } from '../../../../core/models/product';
import { ProductService } from '../../../../core/services/product.service';
import { ProductForm } from '../../components/product-form/product-form';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss',
})
export class ProductList implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly dialog = inject(MatDialog);

  readonly products = signal<Product[]>([]);
  readonly isLoading = signal(true);

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading.set(true);

    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ProductForm, {
      width: '520px',
      maxWidth: '95vw',
      disableClose: true,
      data: {},
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) {
        this.loadProducts();
      }
    });
  }

  openEditDialog(product: Product): void {
    const dialogRef = this.dialog.open(ProductForm, {
      width: '520px',
      maxWidth: '95vw',
      disableClose: true,
      data: {
        product,
      },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) {
        this.loadProducts();
      }
    });
  }

  deleteProduct(product: Product): void {
  const dialogRef = this.dialog.open(ConfirmDialog, {
    width: '460px',
    maxWidth: '95vw',
    data: {
      title: 'Excluir produto',
      message: `Deseja realmente excluir o produto "${product.name}"?`,
      confirmText: 'Excluir',
      cancelText: 'Cancelar'
    }
  });

  dialogRef.afterClosed().subscribe((confirmed) => {
    if (!confirmed) {
      return;
    }

    this.productService.delete(product.id).subscribe({
      next: () => {
        this.loadProducts();
      }
    });
  });
}
}
