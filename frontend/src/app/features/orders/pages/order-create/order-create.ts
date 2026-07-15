import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  inject,
  OnInit,
  signal
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Customer } from '../../../../core/models/customer';
import { CreateOrderRequest } from '../../../../core/models/order';
import { Product } from '../../../../core/models/product';
import { CustomerService } from '../../../../core/services/customer.service';
import { OrderService } from '../../../../core/services/order.service';
import { ProductService } from '../../../../core/services/product.service';

interface SelectedOrderItem {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  availableStock: number;
}

@Component({
  selector: 'app-order-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule
  ],
  templateUrl: './order-create.html',
  styleUrl: './order-create.scss'
})
export class OrderCreate implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly customerService = inject(CustomerService);
  private readonly productService = inject(ProductService);
  private readonly orderService = inject(OrderService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  readonly customers = signal<Customer[]>([]);
  readonly products = signal<Product[]>([]);
  readonly items = signal<SelectedOrderItem[]>([]);

  readonly isLoading = signal(true);
  readonly isSaving = signal(false);

  readonly totalAmount = computed(() =>
    this.items().reduce(
      (total, item) => total + item.quantity * item.unitPrice,
      0
    )
  );

  readonly totalUnits = computed(() =>
    this.items().reduce(
      (total, item) => total + item.quantity,
      0
    )
  );

  readonly orderForm = this.formBuilder.nonNullable.group({
    customerId: ['', Validators.required]
  });

  readonly itemForm = this.formBuilder.nonNullable.group({
    productId: ['', Validators.required],
    quantity: [
      1,
      [
        Validators.required,
        Validators.min(1)
      ]
    ]
  });

  ngOnInit(): void {
    this.loadDependencies();
  }

  loadDependencies(): void {
    this.isLoading.set(true);

    forkJoin({
      customers: this.customerService.getAll(),
      products: this.productService.getAll()
    }).subscribe({
      next: ({ customers, products }) => {
        this.customers.set(customers);
        this.products.set(products);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  addItem(): void {
    if (this.itemForm.invalid) {
      this.itemForm.markAllAsTouched();
      return;
    }

    const { productId, quantity } = this.itemForm.getRawValue();

    const product = this.products().find(
      (item) => item.id === productId
    );

    if (!product) {
      this.showError('Selecione um produto válido.');
      return;
    }

    const existingItem = this.items().find(
      (item) => item.productId === productId
    );

    const newQuantity =
      (existingItem?.quantity ?? 0) + quantity;

    if (newQuantity > product.currentStock) {
      this.showError(
        `Estoque insuficiente. Disponível: ${product.currentStock} unidade(s).`
      );
      return;
    }

    if (existingItem) {
      this.items.update((items) =>
        items.map((item) =>
          item.productId === productId
            ? {
                ...item,
                quantity: newQuantity
              }
            : item
        )
      );
    } else {
      this.items.update((items) => [
        ...items,
        {
          productId: product.id,
          productName: product.name,
          quantity,
          unitPrice: product.price,
          availableStock: product.currentStock
        }
      ]);
    }

    this.itemForm.reset({
      productId: '',
      quantity: 1
    });
  }

  removeItem(productId: string): void {
    this.items.update((items) =>
      items.filter((item) => item.productId !== productId)
    );
  }

  submit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    if (this.items().length === 0) {
      this.showError('Adicione pelo menos um produto ao pedido.');
      return;
    }

    if (this.isSaving()) {
      return;
    }

    this.isSaving.set(true);

    const request: CreateOrderRequest = {
      customerId: this.orderForm.controls.customerId.value,
      items: this.items().map((item) => ({
        productId: item.productId,
        quantity: item.quantity
      }))
    };

    this.orderService.create(request).subscribe({
      next: () => {
        this.snackBar.open(
          'Pedido criado com sucesso.',
          'Fechar',
          {
            duration: 4000,
            horizontalPosition: 'right',
            verticalPosition: 'top'
          }
        );

        void this.router.navigate(['/orders']);
      },
      error: () => {
        this.isSaving.set(false);
      }
    });
  }

  cancel(): void {
    void this.router.navigate(['/orders']);
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Fechar', {
      duration: 5000,
      horizontalPosition: 'right',
      verticalPosition: 'top',
      panelClass: ['error-snackbar']
    });
  }
}